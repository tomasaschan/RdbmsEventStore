namespace RdbmsEventStore
{
    public interface IEventFactory<in TId, out TEvent> where TEvent : IEvent<TId>
    {
        TEvent Create<T>(TId streamId, long version, T payload);
    }
}