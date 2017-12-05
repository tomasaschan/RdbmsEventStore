namespace RdbmsEventStore.Serialization
{
    public interface IPersistedEvent<TStreamId> : IMutableEventMetadata<TStreamId>
    {
        long Version { get; set; }

        string Type { get; set; }

        byte[] Payload { get; set; }
    }
}