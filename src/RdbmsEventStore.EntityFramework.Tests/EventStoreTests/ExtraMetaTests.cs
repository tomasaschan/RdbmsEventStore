using System;
using System.Linq;
using System.Threading.Tasks;
using RdbmsEventStore.EntityFramework.Tests.Infrastructure;
using RdbmsEventStore.EntityFramework.Tests.TestData;
using RdbmsEventStore.EventRegistry;
using RdbmsEventStore.Serialization;
using Xunit;

namespace RdbmsEventStore.EntityFramework.Tests.EventStoreTests
{
    [Collection(nameof(InMemoryDatabaseCollection))]
    public class ExtraMetaTests : IClassFixture<ExtraMetaEventFactoryFixture>
    {
        private readonly ExtraMetaEventFactoryFixture _fixture;
        private readonly EntityFrameworkEventStoreContext<ExtraMetaLongStringPersistedEventMetadata> _dbContext;

        // ReSharper disable once UnusedParameter.Local
        public ExtraMetaTests(ExtraMetaEventFactoryFixture fixture, AssemblyInitializerFixture _)
        {
            EffortProviderFactory.ResetDb();
            _fixture = fixture;
            _dbContext = new EntityFrameworkEventStoreContext<ExtraMetaLongStringPersistedEventMetadata>();

            var stream1 = _fixture.EventFactory.Create("stream-1", new object[] {
                    new FooEvent { Foo = "Foo" },
                    new BarEvent { Bar = "Bar" },
                    new FooEvent { Foo = "Baz" }
                })
                .Select(_fixture.EventSerializer.Serialize);
            var stream2 = _fixture.EventFactory.Create("stream-2", new object[] {
                    new FooEvent { Foo = "Boo" },
                    new BarEvent { Bar = "Far" }
                })
                .Select(_fixture.EventSerializer.Serialize);

            _dbContext.Events.AddRange(stream1);
            _dbContext.Events.AddRange(stream2);
            _dbContext.SaveChanges();
        }

        [Theory]
        [InlineData("stream-1", 3)]
        [InlineData("stream-2", 2)]
        public async Task ReturnsEventsFromCorrectStreamOnly(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, ExtraMetaStringEvent, IExtraMeta>;
            var events = await store.Events(streamId);
            Assert.Equal(expectedCount, events.Count());
        }

        [Theory]
        [InlineData("stream-1", 2)]
        [InlineData("stream-2", 1)]
        public async Task ReturnsEventsAccordingToQuery(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, ExtraMetaStringEvent, IExtraMeta>;
            var events = await store.Events(streamId, es => es.Where(e => e.ExtraMeta.StartsWith("Foo")));
            Assert.Equal(expectedCount, events.Count());
        }

        [Theory]
        [InlineData("stream-1", 2)]
        [InlineData("stream-2", 1)]
        public async Task ReturnsEventsWithMetadata(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, ExtraMetaStringEvent, IExtraMeta>;
            var events = await store
                .Events(streamId, es => es.Where(e => e.ExtraMeta.StartsWith("Foo")))
                .ToReadOnlyCollection();

            Assert.Equal(expectedCount, events.Count);
            Assert.All(events, @event => Assert.StartsWith("Foo", @event.ExtraMeta));
        }

        [Theory]
        [InlineData("stream-1", 2)]
        [InlineData("stream-2", 1)]
        public async Task CanQueryByExtraMetadata(string streamId, long expectedCount)
        {
            var store = _fixture.BuildEventStore(_dbContext) as IEventStore<string, ExtraMetaStringEvent, IExtraMeta>;
            var events = await store.Events(streamId, es => es.Where(e => e.ExtraMeta.StartsWith("Foo")));
            Assert.Equal(expectedCount, events.Count());
        }
    }

    public class ExtraMetaEventFactory : DefaultEventFactory<string, ExtraMetaStringEvent>
    {
        private int _total;

        protected override ExtraMetaStringEvent CreateSingle(string streamId, object payload)
        {
            var @event = base.CreateSingle(streamId, payload);
            @event.ExtraMeta = $"{payload.GetType().Name}-{_total++}";
            return @event;
        }
    }

    public class ExtraMetaEventSerializer : DefaultEventSerializer<string, ExtraMetaStringEvent, ExtraMetaLongStringPersistedEventMetadata>
    {
        public ExtraMetaEventSerializer(IEventRegistry registry) : base(registry)
        {
        }

        public override ExtraMetaLongStringPersistedEventMetadata Serialize(ExtraMetaStringEvent @event)
        {
            var serialized = base.Serialize(@event);
            serialized.ExtraMeta = @event.ExtraMeta;
            return serialized;
        }

        public override ExtraMetaStringEvent Deserialize(ExtraMetaLongStringPersistedEventMetadata @event)
        {
            var deserialized = base.Deserialize(@event);
            deserialized.ExtraMeta = @event.ExtraMeta;
            return deserialized;
        }
    }
}