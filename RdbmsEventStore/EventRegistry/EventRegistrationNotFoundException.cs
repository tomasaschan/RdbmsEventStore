using System;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class EventNameNotFoundException : EventRegistrationNotFoundException
    {
        public EventNameNotFoundException(Type soughtType, IReadOnlyDictionary<Type, string> registry)
            : base($"Event type '{soughtType.AssemblyQualifiedName}' was not present in the registry.", registry.ToDictionary(kvp => kvp.Value, kvp => kvp.Key))
        {
            SoughtType = soughtType;
        }

        public Type SoughtType { get; }
    }

    public class EventTypeNotFoundException : EventRegistrationNotFoundException
    {
        public EventTypeNotFoundException(string soughtType, IReadOnlyDictionary<string, Type> registry)
            : base($"Event type '{soughtType}' was not present in the registry.", registry)
        {
            SoughtType = soughtType;
        }

        public string SoughtType { get; }
    }

    public class EventRegistrationNotFoundException : InvalidOperationException
    {
        protected EventRegistrationNotFoundException(string message, IReadOnlyDictionary<string, Type> registry) : base(message)
        {
            Registry = registry;
        }

        public IReadOnlyDictionary<string, Type> Registry { get; }
    }
}