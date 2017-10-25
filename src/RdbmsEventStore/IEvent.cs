using System;

namespace RdbmsEventStore
{
    public interface IEvent<out TStreamId> : IEventMetadata<TStreamId>
    {
        Type Type { get; }

        object Payload { get; }
    }
}