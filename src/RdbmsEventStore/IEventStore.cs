using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore
{
    public interface IEventStore<in TId, in TStreamId, TEvent> : IEventStream<TId, TStreamId, TEvent>, IEventWriter<TId, TStreamId, TEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : IEvent<TId, TStreamId>
    { }

    public interface IEventStream<in TId, in TStreamId, TEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : IEvent<TId, TStreamId>
    {
        Task<IEnumerable<TEvent>> Events(TStreamId streamId);

        Task<IEnumerable<TEvent>> Events(TStreamId streamId, Func<IQueryable<TEvent>, IQueryable<TEvent>> query);
    }

    public interface IEventWriter<in TId, in TStreamId, in TEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TEvent : IEvent<TId, TStreamId>
    {
        Task Commit(TStreamId streamId, long versionBefore, params object[] payloads);
    }
}