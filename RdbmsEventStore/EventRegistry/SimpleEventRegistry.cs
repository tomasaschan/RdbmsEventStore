using System;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class SimpleEventRegistry : IEventRegistry
    {
        private readonly IReadOnlyDictionary<Type, string> _typeToString;
        public SimpleEventRegistry(IReadOnlyDictionary<string, Type> eventTypes)
        {
            Registrations = eventTypes;
            _typeToString = eventTypes
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        public Type TypeFor(string eventType)
            => Registrations.TryGetValue(eventType, out var type)
                ? type
                : throw new EventTypeNotFoundException(eventType, Registrations);

        public string NameFor(Type eventType)
            =>_typeToString.TryGetValue(eventType, out var name)
            ? name
            : throw new EventNameNotFoundException(eventType, _typeToString);

        public IReadOnlyDictionary<string, Type> Registrations { get; }
    }
}