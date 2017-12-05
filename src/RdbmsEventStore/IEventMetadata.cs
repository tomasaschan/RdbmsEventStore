using System;

namespace RdbmsEventStore
{
    public interface IEventMetadata<out TStreamId>
    {
        DateTimeOffset Timestamp { get; }

        TStreamId StreamId { get; }
    }
}