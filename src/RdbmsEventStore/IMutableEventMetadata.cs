using System;

namespace RdbmsEventStore
{
    public interface IMutableEventMetadata<TStreamId> : IEventMetadata<TStreamId>
    {
        new DateTimeOffset Timestamp { get; set; }

        new TStreamId StreamId { get; set; }

        new long Version { get; set; }
    }
}