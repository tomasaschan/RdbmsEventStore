namespace RdbmsEventStore.Serialization
{
    public interface IEventSerializer<TEvent, TPersistedEvent>
    {
        TPersistedEvent Serialize(TEvent @event);

        TEvent Deserialize(TPersistedEvent @event);
    }
}