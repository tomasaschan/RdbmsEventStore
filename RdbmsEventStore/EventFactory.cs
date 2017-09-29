using System;
using System.IO;
using System.Text;

namespace RdbmsEventStore
{
    public class EventFactory<TId, TEvent> : EventFactoryBase, IEventFactory<TId, TEvent>
        where TEvent : IMutableEvent<TId>, new()
    {
        public EventFactory(IEventRegistry registry) : base(registry)
        {
        }

        public virtual TEvent Create<T>(TId streamId, long version, T payload)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, payload);
            return new TEvent
            {
                StreamId = streamId,
                Timestamp = DateTimeOffset.UtcNow,
                Version = version,
                Type = _registry.NameFor(typeof(T)),
                Payload = Encoding.UTF8.GetBytes(writer.ToString())
            };
        }
    }
}