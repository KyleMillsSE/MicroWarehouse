using System;

namespace EventBusAWSSQS
{
    public class EventReceivedArgs : EventArgs
    {
        public string EventType { get; }
        public string Data { get; }
        public string ReceiptHandle { get; }

        public EventReceivedArgs(string eventType, string data, string receiptHandle)
        {
            EventType = eventType;
            Data = data;
            ReceiptHandle = receiptHandle;
        }
    }
}
