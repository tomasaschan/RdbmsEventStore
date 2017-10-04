using System;
using System.Collections.Generic;
using System.Linq;
using RdbmsEventStore.EventRegistry;

namespace RdbmsEventStore
{
    public class DefaultMaterializer : IMaterializer
    {
        private readonly IEventRegistry _registry;
        private readonly IEventSerializer _serializer;

        public DefaultMaterializer(IEventRegistry registry, IEventSerializer serializer)
        {
            _registry = registry;
            _serializer = serializer;
        }

        public TState Unfold<TState, TId, TStreamId>(TState initialState, IEnumerable<IEvent<TId, TStreamId>> events, Func<TState, object, TState> applicator)
            => events
                .Select(@event =>
                {
                    var payload = @event.Payload;
                    try
                    {
                        var type = _registry.TypeFor(@event.Type);
                        return _serializer.Deserialize(payload, type);
                    }
                    catch (EventRegistrationNotFoundException ex)
                    {
                        throw new MaterializationException(@event.Type, _serializer.Show(payload), ex);
                    }
                })
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