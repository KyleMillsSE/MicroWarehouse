using System.Collections.Generic;
using MessageAttributeValue = Amazon.SimpleNotificationService.Model.MessageAttributeValue;

namespace EventBusAWSSQS
{
    // There is probably a types for this already however not found it
    public class SNSNotficationMessage
    {
        public string Value { get; set; }
    }

    public class SNSNotificationMessage
    {
        public string Message { get; set; }
        public Dictionary<string, SNSNotficationMessage> MessageAttributes { get; set; }
    }
}
