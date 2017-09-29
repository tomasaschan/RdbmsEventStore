using System;
using System.IO;
using System.Text;

namespace RdbmsEventStore
{
    public class EventFactory<TId, TEvent> : EventFactoryBase, IEventFactory<TId, TEvent>
        where TEvent : IMutableEvent<TId>, new()
    {
        public virtual TEvent Create<T>(TId streamId, long version, T payload)
        {
            var writer = new StringWriter();
            serializer.Serialize(writer, payload);
            return new TEvent
            {
                StreamId = streamId,
                Timestamp = DateTimeOffset.UtcNow,
                Version = version,
                Type = typeof(T).AssemblyQualifiedName,
                Payload = Encoding.UTF8.GetBytes(writer.ToString())
            };
        }
    }
}