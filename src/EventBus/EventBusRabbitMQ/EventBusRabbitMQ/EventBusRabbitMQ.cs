using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        // Dependencies
        private readonly ILogger<EventBusRabbitMQ> _logger;
        private readonly IServiceProvider _serviceProvider;

        // These should be set via configuration or options pattern
        private readonly string _brokerName;
        private readonly string _brokerType;
        private readonly string _hostName;
        private readonly string _queueName;

        // These should be abstracted away into their own connection poco
        private IConnectionFactory _factory;

        private IConnection _connection;
        private IConnection Connection
        {
            get
            {
                if (_connection == null)
                {
                    _factory = new ConnectionFactory()
                    {
                        HostName = _hostName,
                    };
                    _connection = _factory.CreateConnection();
                }

                return _connection;
            }
        }

        private IModel _consumeChannel;
        private IModel ConsumeChannel
        {
            get
            {
                // Initialise the first time using
                if (_consumeChannel == null)
                {
                    _consumeChannel = Connection.CreateModel();

                    // Declare exchange
                    _consumeChannel.ExchangeDeclare(_brokerName, ExchangeType.Direct);

                    // Declare queue
                    _consumeChannel.QueueDeclare(_queueName, false, false, false, null);
                }

                return _consumeChannel;
            }
        }

        // Should be abstracted away
        private readonly Dictionary<string, (Type EventType, Type CallbackType)> _subscriptions;

        // Local flags 
        private bool _consumingStarted;
        private bool _disposed;

        public EventBusRabbitMQ(ILogger<EventBusRabbitMQ> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;

            _brokerName = "micro_deft_broker_v3";
            _hostName = "localhost";
            _queueName = "micro_deft_queue_v3_" + Guid.NewGuid().ToString().Replace("-", "");

            _subscriptions = new Dictionary<string, (Type, Type)>();
            _consumingStarted = false;
            _disposed = false;
        }

        // Limitation event can only be subscribed to once however same call back can used multiple times
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

            // Bind routing key to queue allowing to accept messages of these keys
            ConsumeChannel.QueueBind(_queueName, _brokerName, eventName);

            // If not consuming set up event handler for receiving
            if (!_consumingStarted)
            {
                var consumer = new EventingBasicConsumer(ConsumeChannel);

                consumer.Received += Consumer_Received;

                ConsumeChannel.BasicConsume(_queueName, false, consumer);

                _consumingStarted = true;
            }
        }

        public void Publish(Event @event)
        {
            var jsonData = JsonConvert.SerializeObject(@event);

            var encodedData = Encoding.UTF8.GetBytes(jsonData);

            using (var publishChannel = Connection.CreateModel())
            {
                publishChannel.ExchangeDeclare(_brokerName, ExchangeType.Direct);

                var properties = publishChannel.CreateBasicProperties();

                properties.DeliveryMode = 2; 

                _logger.LogInformation($"Publishing event '{@event.Id}' to RabbitMQ");

                publishChannel.BasicPublish(_brokerName, @event.GetType().Name, null, encodedData);
            }
        }

        private void Consumer_Received(object sender, BasicDeliverEventArgs eventArgs)
        {
            var eventName = eventArgs.RoutingKey;
            var encodedData = Encoding.UTF8.GetString(eventArgs.Body);

            ExecuteCallBack(eventName, encodedData);

            ConsumeChannel.BasicAck(eventArgs.DeliveryTag, false);
        }

        private void ExecuteCallBack(string eventName, string encodedData)
        {
            // Check if event has been subscribed for if yes then execute callback
            if (_subscriptions.TryGetValue(eventName, out var subscription))
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var @event = (Event)JsonConvert.DeserializeObject(encodedData, subscription.EventType);

                    var callBack = (IEventCallback)scope.ServiceProvider.GetService(subscription.CallbackType);

                    callBack.Execute(@event);
                }
            }
        }

        #region Dispose

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _consumeChannel?.Dispose();
                _connection?.Dispose();
                _factory = null;
            }

            _disposed = true;
        }

        #endregion

    }
}
