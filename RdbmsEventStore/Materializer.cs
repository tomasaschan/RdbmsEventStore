using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RdbmsEventStore.EventRegistry;

namespace RdbmsEventStore
{
    public class Materializer : IMaterializer
    {
        private readonly IEventRegistry _registry;

        public Materializer(IEventRegistry registry)
        {
            _registry = registry;
        }

        private object DeserializeEvent<TId>(IEvent<TId> @event)
        {
            var payload = Encoding.UTF8.GetString(@event.Payload);
            try
            {
                var type = _registry.TypeFor(@event.Type);
                return JsonConvert.DeserializeObject(payload, type);
            }
            catch (EventRegistrationNotFoundException ex)
            {
                throw new MaterializationException(@event.Type, payload, ex);
            }
        }

        public TState Unfold<TState, TId>(TState initialState, IEnumerable<IEvent<TId>> events, Func<TState, object, TState> applicator)
            => events
                .Select(DeserializeEvent)
                .Aggregate(initialState, applicator);
    }

    public class MaterializationException : InvalidOperationException
    {
        public MaterializationException(string type, string payload, Exception innerException)
            : base($"Unable to deserialize '{payload}' into event of type '{type}'.", innerException)
        {
        }
    }
}