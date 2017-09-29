using System;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class SimpleEventRegistry : IEventRegistry
    {
        private readonly IReadOnlyDictionary<string, Type> _stringToType;
        private readonly IReadOnlyDictionary<Type, string> _typeToString;

        public SimpleEventRegistry(IReadOnlyDictionary<string, Type> eventTypes)
        {
            _stringToType = eventTypes;
            _typeToString = eventTypes
                .ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
        }

        public Type TypeFor(string eventType)
            => _stringToType.TryGetValue(eventType, out var type)
                ? type
                : throw new EventTypeNotFoundException(eventType, _stringToType);

        public string NameFor(Type eventType)
            =>_typeToString.TryGetValue(eventType, out var name)
            ? name
            : throw new EventNameNotFoundException(eventType, _typeToString);
    }
}