using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.Serialization;

namespace RdbmsEventStore.EntityFramework
{
    public class EntityFrameworkEventStore<TId, TStreamId, TContext, TEvent, TEventMetadata, TPersistedEvent> : IEventStream<TStreamId, TEvent, TEventMetadata>
        where TId : IEquatable<TId>
        where TStreamId : IEquatable<TStreamId>
        where TContext : DbContext, IEventDbContext<TPersistedEvent>
        where TEvent : class, TEventMetadata, IMutableEvent<TStreamId>, new()
        where TPersistedEvent : class, IPersistedEvent<TStreamId>, TEventMetadata, new()
        where TEventMetadata : IEventMetadata<TStreamId>
    {
        private readonly TContext context;
        private readonly IEventFactory<TStreamId, TEvent> _eventFactory;
        private readonly IWriteLock _writeLock;
        private readonly IEventSerializer<TEvent, TPersistedEvent> _serializer;

        public EntityFrameworkEventStore(TContext context, IEventFactory<TStreamId, TEvent> eventFactory, IWriteLock writeLock, IEventSerializer<TEvent, TPersistedEvent> serializer)
        {
            this.context = context;
            _eventFactory = eventFactory;
            _writeLock = writeLock;
            _serializer = serializer;
        }

        public Task<IEnumerable<TEvent>> Events(TStreamId streamId) => Events(streamId, query => query);

        public async Task<IEnumerable<TEvent>> Events(TStreamId streamId, Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
        {
            var storedEvents = await context.Events
                    .Where(e => e.StreamId.Equals(streamId))
                    .AsNoTracking()
                    .Apply(query)
                    .ToListAsync();
            var events = storedEvents
                .Cast<TPersistedEvent>()
                .Select(_serializer.Deserialize);

            return events;
        }

        public Task Commit(TStreamId streamId, long versionBefore, object payload)
            => Commit(streamId, versionBefore, new[] { payload });

        public async Task Commit(TStreamId streamId, long versionBefore, IEnumerable<object> payloads)
        {
            using (await _writeLock.Aquire())
            {
                var highestVersionNumber = await context.Events
                    .Where(e => e.StreamId.Equals(streamId))
                    .Select(e => e.Version)
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                if (highestVersionNumber != versionBefore)
                {
                    throw new ConflictException(streamId, versionBefore, highestVersionNumber, payloads);
                }

                var events = _eventFactory.Create(streamId, versionBefore, payloads).Select(_serializer.Serialize);
                context.Events.AddRange(events);
                await context.SaveChangesAsync();
            }
        }
    }
}
