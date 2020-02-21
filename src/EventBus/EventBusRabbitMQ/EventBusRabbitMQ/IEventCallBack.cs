namespace EventBusRabbitMQ
{
    public interface IEventCallback
    {
        void Execute(Event @event);
    }
}
