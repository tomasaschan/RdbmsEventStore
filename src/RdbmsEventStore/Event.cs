using System;

namespace RdbmsEventStore
{
    public class Event<TStreamId> : IMutableEvent<TStreamId>
    {
        public DateTimeOffset Timestamp { get; set; }
        public TStreamId StreamId { get; set; }
        public long Version { get; set; }
        public Type Type { get; set; }
        public object Payload { get; set; }
    }
}