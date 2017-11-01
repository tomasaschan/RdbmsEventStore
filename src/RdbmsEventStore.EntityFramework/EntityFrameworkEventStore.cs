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
        where TEventMetadata : class, IEventMetadata<TStreamId>
    {
        private readonly TContext _context;
        private readonly IEventFactory<TStreamId, TEvent> _eventFactory;
        private readonly IWriteLock<TStreamId> _writeLock;
        private readonly IEventSerializer<TEvent, TPersistedEvent> _serializer;

        public EntityFrameworkEventStore(TContext context, IEventFactory<TStreamId, TEvent> eventFactory, IWriteLock<TStreamId> writeLock, IEventSerializer<TEvent, TPersistedEvent> serializer)
        {
            _context = context;
            _eventFactory = eventFactory;
            _writeLock = writeLock;
            _serializer = serializer;
        }

        public  Task<IEnumerable<TEvent>> Events() => Events(events => events);

        public async Task<IEnumerable<TEvent>> Events(Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
        {
            var storedEvents = await _context.Events
                .AsNoTracking()
                .Apply(query)
                .OrderBy(e => e.Timestamp)
                .ToListAsync();

            var events = storedEvents
                .Cast<TPersistedEvent>()
                .Select(_serializer.Deserialize);

            return events;
        }

        public Task<IEnumerable<TEvent>> Events(TStreamId streamId) => Events(streamId, events => events);

        public Task<IEnumerable<TEvent>> Events(TStreamId streamId, Func<IQueryable<TEventMetadata>, IQueryable<TEventMetadata>> query)
            => Events(events => events.Where(e => e.StreamId.Equals(streamId)).Apply(query));

        public Task Append(TStreamId streamId, long versionBefore, object payload)
            => Append(streamId, versionBefore, new[] { payload });

        public async Task Append(TStreamId streamId, long versionBefore, IEnumerable<object> payloads)
        {
            using (await _writeLock.Aquire(streamId))
            {
                var highestVersionNumber = await _context.Events
                    .Where(e => e.StreamId.Equals(streamId))
                    .Select(e => e.Version)
                    .DefaultIfEmpty(0)
                    .MaxAsync();

                if (highestVersionNumber != versionBefore)
                {
                    throw new ConflictException(streamId, versionBefore, highestVersionNumber, payloads);
                }

                var events = _eventFactory.Create(streamId, versionBefore, payloads).Select(_serializer.Serialize);
                _context.Events.AddRange(events);
                await _context.SaveChangesAsync();
            }
        }
    }
}
