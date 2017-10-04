using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace RdbmsEventStore.EntityFramework
{
    public class EntityFrameworkEventStore<TId, TStreamId, TContext, TEvent> : IEventStore<TId, TStreamId, TEvent>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TContext : DbContext, IEventDbContext<TEvent>
        where TEvent : Event<TId, TStreamId>, IEvent<TId, TStreamId>, new()
    {
        private readonly TContext context;
        private readonly IEventFactory<TId, TStreamId, TEvent> _eventFactory;
        private readonly IWriteLock _writeLock;

        public EntityFrameworkEventStore(TContext context, IEventFactory<TId, TStreamId, TEvent> eventFactory, IWriteLock writeLock)
        {
            this.context = context;
            _eventFactory = eventFactory;
            _writeLock = writeLock;
        }

        public Task<IEnumerable<TEvent>> Events(TStreamId streamId) => Events(streamId, query => query);

        public async Task<IEnumerable<TEvent>> Events(TStreamId streamId, Func<IQueryable<TEvent>, IQueryable<TEvent>> query)
            => await context.Events
                .Where(e => e.StreamId.Equals(streamId))
                .Apply(query)
                .AsNoTracking()
                .ToListAsync();

        public async Task Commit(TStreamId streamId, long versionBefore, params object[] payloads)
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
                    throw new ConflictException(streamId, versionBefore, highestVersionNumber, payloads);
                }

                var events = _eventFactory.Create(streamId, versionBefore, payloads);
                context.Events.AddRange(events);
                await context.SaveChangesAsync();
            }
        }
    }
}
