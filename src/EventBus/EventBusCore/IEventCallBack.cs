namespace EventBusCore
{
    public interface IEventCallback
    {
        void Execute(Event @event);
    }
}
