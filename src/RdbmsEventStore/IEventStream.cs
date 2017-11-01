using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore
{
    public interface IEventStream<in TStreamId, TEvent, TEventMetadata>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : IEvent<TStreamId>, TEventMetadata
        where TEventMetadata : IEventMetadata<TStreamId>
    {
        Task<IEnumerable<TEvent>> Events(Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query);

        Task Append(TStreamId streamId, long versionBefore, IEnumerable<object> payloads);
    }
}