namespace RdbmsEventStore.Serialization
{
    public interface IPersistedEvent<TStreamId> : IMutableEventMetadata<TStreamId>
    {
        string Type { get; set; }

        byte[] Payload { get; set; }
    }
}