using System;

namespace RdbmsEventStore
{
    public interface IMutableEvent<TStreamId> : IMutableEventMetadata<TStreamId>, IEvent<TStreamId>
    {
        new Type Type { get; set; }

        new object Payload { get; set; }
    }
}