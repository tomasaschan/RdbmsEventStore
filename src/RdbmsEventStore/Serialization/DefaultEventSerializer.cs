using System;
using System.IO;
using System.Text;
using RdbmsEventStore.EventRegistry;

namespace RdbmsEventStore.Serialization
{
    public class DefaultEventSerializer<TStreamId, TEvent, TPersistedEvent> : JsonEventSerializer, IEventSerializer<TEvent, TPersistedEvent>
        where TPersistedEvent : IPersistedEvent<TStreamId>, new()
        where TEvent : IMutableEvent<TStreamId>, new()
    {
        private readonly IEventRegistry _registry;

        public DefaultEventSerializer(IEventRegistry registry)
        {
            _registry = registry;
        }

        public virtual TPersistedEvent Serialize(TEvent @event)
        {
            var type = _registry.NameFor(@event.Type);
            var payload = SerializePayload(@event.Payload);

            return new TPersistedEvent
            {
                StreamId = @event.StreamId,
                Timestamp = @event.Timestamp,
                Type = type,
                Payload = payload
            };

        }

        public virtual TEvent Deserialize(TPersistedEvent @event)
        {
            var type = _registry.TypeFor(@event.Type);
            var payload = DeserializePayload(@event.Payload, type);

            return new TEvent
            {
                StreamId = @event.StreamId,
                Timestamp = @event.Timestamp,
                Type = type,
                Payload = payload
            };
        }

        private static byte[] SerializePayload(object payload)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, payload);
            return Encoding.UTF8.GetBytes(writer.ToString());
        }

        private static object DeserializePayload(byte[] data, Type type)
        {
            var reader = new StringReader(Encoding.UTF8.GetString(data));
            return serializer.Deserialize(reader, type);
        }
    }
}