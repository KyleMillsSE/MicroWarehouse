using Amazon.SQS;
using Amazon.SQS.Model;
using EventBusCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Newtonsoft.Json.Linq;
using MessageAttributeValue = Amazon.SimpleNotificationService.Model.MessageAttributeValue;

namespace EventBusAWSSQS
{
    public sealed class EventBusAWSSQS : IEventBus
    {
        private const string EVENT_TYPE = "Micro_EventType";

        // Dependencies
        private readonly ILogger<EventBusAWSSQS> _logger;
        private readonly IServiceProvider _serviceProvider;

        // Configuration - should be options
        private readonly string _topic;
        private string _queuePrefix;

        // AWS services 
        private readonly AmazonSQSClient _amazonSQSClient;
        private readonly AmazonSimpleNotificationServiceClient _amazonSNSClient;

        // Polling
        private CancellationTokenSource _pollingQueueCancellationTokenSource;
        private Thread _pollingQueueThread;

        // Event handler to be executed
        private event EventHandler<EventReceivedArgs> OnEventReceived;

        // Local flags 
        private bool _consumingStarted;

        // Should be abstracted away
        private readonly Dictionary<string, (Type EventType, Type CallbackType)> _subscriptions;

        private string _queueUrl;
        private string QueueUrl
        {
            get
            {
                if (_queueUrl == null)
                {
                    var createQueueRequest = new CreateQueueRequest
                    {
                        QueueName = _queuePrefix,
                        Attributes = new Dictionary<string, string>()
                        {
                            { "FifoQueue", "true" },
                            { "ReceiveMessageWaitTimeSeconds", "20"}
                        },
                    };

                    var createQueueResponse = _amazonSQSClient.CreateQueueAsync(createQueueRequest).GetAwaiter().GetResult();

                    if (createQueueResponse.HttpStatusCode != HttpStatusCode.OK)
                    {
                        throw new Exception("Error occurred creating queue");
                    }

                    _queueUrl = createQueueResponse.QueueUrl;

                    _amazonSNSClient.SubscribeQueueAsync(_topic, _amazonSQSClient, createQueueResponse.QueueUrl).GetAwaiter().GetResult();

                    // Sleep to wait for the subscribe to complete.
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                }

                return _queueUrl;
            }
        }

        public EventBusAWSSQS(ILogger<EventBusAWSSQS> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            _topic = "arn:aws:sns:us-east-1:000000000000:MicroTopic";
            _queuePrefix = "micro_deft_queue_v3_AWSSQS_" + Guid.NewGuid().ToString().Replace("-", "");

            _amazonSQSClient = new AmazonSQSClient(new AmazonSQSConfig { ServiceURL = "http://localhost:4100" });
            _amazonSNSClient = new AmazonSimpleNotificationServiceClient(new AmazonSimpleNotificationServiceConfig() { ServiceURL = "http://localhost:4100" });

            _subscriptions = new Dictionary<string, (Type, Type)>();
            _consumingStarted = false;
        }

        public void Subscribe<TEvent, TEventCallback>()
            where TEvent : Event
            where TEventCallback : IEventCallback
        {
            var eventName = typeof(TEvent).Name;

            if (_subscriptions.ContainsKey(eventName))
            {
                _logger.LogWarning($"A callback for event '{eventName}' has already been subscribed");

                return;
            }

            _subscriptions.Add(eventName, (typeof(TEvent), typeof(TEventCallback)));

            // If not consuming set up event handler for receiving
            if (!_consumingStarted)
            {
                // start some sort of thread and event handle
                OnEventReceived += async (s, e) => await Consumer_Received(s, e);

                _pollingQueueCancellationTokenSource = new CancellationTokenSource();

                _pollingQueueThread = new Thread(async (object obj) => await ConsumerStartLongPolling(obj));
                _pollingQueueThread.Start(_pollingQueueCancellationTokenSource.Token);

                _consumingStarted = true;
            }
        }

        public void Publish(Event @event)
        {
            // Publish the message to the topic
            var publishMessageResponse = _amazonSNSClient.PublishAsync(new PublishRequest
            {
                TopicArn = _topic,
                Message = JsonConvert.SerializeObject(@event),
                MessageAttributes = new Dictionary<string, MessageAttributeValue>()
                {
                    { EVENT_TYPE, new MessageAttributeValue() {  DataType = "String",StringValue = @event.GetType().Name } },
                }
            }).GetAwaiter().GetResult(); // gross

            if (publishMessageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error occurred publishing messaging using SNS e with status code '{publishMessageResponse.HttpStatusCode}'");
            }
        }

        private async Task ConsumerStartLongPolling(object obj)
        {
            var cancellationToken = (CancellationToken)obj;

            while (!cancellationToken.IsCancellationRequested)
            {
                using (var ctx = new CancellationTokenSource())
                using (var ctsForPolling = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, ctx.Token))
                {
                    var request = new ReceiveMessageRequest
                    {
                        AttributeNames = { "SentTimestamp", EVENT_TYPE },
                        MaxNumberOfMessages = 1,
                        MessageAttributeNames = { "All" },
                        QueueUrl = QueueUrl,
                        WaitTimeSeconds = 20
                    };

                    var receiveMessageResponse = await _amazonSQSClient.ReceiveMessageAsync(request, ctsForPolling.Token);

                    if (receiveMessageResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        var message = receiveMessageResponse.Messages.FirstOrDefault();

                        if (message != null)
                        {
                            var snsNotifcationMessage = JsonConvert.DeserializeObject<SNSNotificationMessage>(message.Body);

                            // ReSharper disable once PossibleNullReferenceException
                            OnEventReceived.Invoke(this, new EventReceivedArgs(snsNotifcationMessage.MessageAttributes[EVENT_TYPE].Value, snsNotifcationMessage.Message, message.ReceiptHandle));
                        }
                    }
                    else
                    {
                        _logger.LogError($"Error occurred receiving message from SQS queue with status code '{receiveMessageResponse.HttpStatusCode}'");
                    }
                }
            }
        }

        private async Task Consumer_Received(object sender, EventReceivedArgs eventArgs)
        {
            ExecuteCallBack(eventArgs.EventType, eventArgs.Data);

            var deleteMessageRequest = new DeleteMessageRequest
            {
                QueueUrl = QueueUrl,
                ReceiptHandle = eventArgs.ReceiptHandle
            };

            var deleteMessageResponse = await _amazonSQSClient.DeleteMessageAsync(deleteMessageRequest);

            if (deleteMessageResponse.HttpStatusCode != HttpStatusCode.OK)
            {
                _logger.LogError($"Error occurred deleting message from SQS queue with status code '{deleteMessageResponse.HttpStatusCode}'");
            }
        }

        private void ExecuteCallBack(string eventName, string decodedData)
        {
            // Check if event has been subscribed for if yes then execute callback
            if (_subscriptions.TryGetValue(eventName, out var subscription))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var @event = (Event)JsonConvert.DeserializeObject(decodedData, subscription.EventType);

                    var callBack = (IEventCallback)scope.ServiceProvider.GetService(subscription.CallbackType);

                    callBack.Execute(@event);
                }
            }
        }
    }
}
