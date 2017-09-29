using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore
{
    public interface IEventStore<in TId, TEvent> : IEventStream<TId, TEvent>, IEventWriter<TId, TEvent>
        where TId : IEquatable<TId>
        where TEvent : IEvent<TId>
    { }

    public interface IEventStream<in TId, TEvent>
        where TId : IEquatable<TId>
        where TEvent : IEvent<TId>
    {
        Task<IEnumerable<TEvent>> Events(TId streamId);

        Task<IEnumerable<TEvent>> Events(TId streamId, Func<IQueryable<TEvent>, IQueryable<TEvent>> query);
    }

    public interface IEventWriter<in TId, in TEvent>
        where TId : IEquatable<TId>
        where TEvent : IEvent<TId>
    {
        Task Commit(TId streamId, long versionBefore, params object[] payloads);
    }
}