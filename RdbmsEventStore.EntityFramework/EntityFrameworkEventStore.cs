using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore.EntityFramework
{
    public class EntityFrameworkEventStore<TId, TContext, TEvent> : IEventStore<TId, TEvent>
        where TId : IEquatable<TId>
        where TContext : DbContext, IEventDbContext<TEvent>
        where TEvent : Event<TId>, IEvent<TId>, new()
    {
        private readonly TContext context;
        private readonly IEventFactory<TId, TEvent> _eventFactory;
        private readonly IWriteLock _writeLock;

        public EntityFrameworkEventStore(TContext context, IEventFactory<TId, TEvent> eventFactory, IWriteLock writeLock)
        {
            this.context = context;
            _eventFactory = eventFactory;
            _writeLock = writeLock;
        }

        public Task<IEnumerable<TEvent>> Events(TId streamId) => Events(streamId, query => query);

        public async Task<IEnumerable<TEvent>> Events(TId streamId, Func<IQueryable<TEvent>, IQueryable<TEvent>> query)
            => await context.Events
                .Where(e => e.StreamId.Equals(streamId))
                .Apply(query)
                .AsNoTracking()
                .ToListAsync();

        public async Task Commit(TId streamId, long versionBefore, params object[] payloads)
        {
            using (await _writeLock.Aquire())
            {
                var highestVersionNumber = await context.Events
                    .Where(e => e.StreamId.Equals(streamId))
                    .Select(e => e.Version)
                    .DefaultIfEmpty(0L)
                    .MaxAsync();

                if (highestVersionNumber != versionBefore)
                {
                    // TODO: throw conflict exception
                }

                var events = _eventFactory.Create(streamId, versionBefore, payloads);
                context.Events.AddRange(events);
                await context.SaveChangesAsync();
            }
        }
    }
}
