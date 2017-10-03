using System;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class SimpleEventRegistry : IComposableEventRegistry
    {
        private readonly IReadOnlyDictionary<Type, string> _typeToString;
        public SimpleEventRegistry(IEnumerable<Type> eventTypes, Func<Type, string> namer)
            : this(eventTypes.Select(type => new KeyValuePair<string, Type>(namer(type), type)))
        {
        }

        public SimpleEventRegistry(IEnumerable<KeyValuePair<string, Type>> registrations)
        {
            try
            {
                Registrations = registrations
                    .ToDictionary(registration => registration.Key, registration => registration.Value);
                _typeToString = Registrations
                    .ToDictionary(registration => registration.Value, kvp => kvp.Key);
            }
            catch (ArgumentException ex)
            {
                throw new EventTypeRegistrationException("Invalid event type registration; two or more types are either named the same, or registered twice", ex);
            }
        }

        public Type TypeFor(string eventType)
            => Registrations.TryGetValue(eventType, out var type)
                ? type
                : throw new EventTypeNotFoundException(eventType, Registrations);

        public string NameFor(Type eventType)
            => _typeToString.TryGetValue(eventType, out var name)
            ? name
            : throw new EventNameNotFoundException(eventType, _typeToString);

        public IReadOnlyDictionary<string, Type> Registrations { get; }
    }
}