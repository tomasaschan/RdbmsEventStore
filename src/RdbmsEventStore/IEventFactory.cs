using System.Collections.Generic;

namespace RdbmsEventStore
{
    public interface IEventFactory<in TId, in TStreamId, out TEvent> where TEvent : IEvent<TId, TStreamId>
    {
        IEnumerable<TEvent> Create(TStreamId streamId, long version, params object[] payloads);
    }
}