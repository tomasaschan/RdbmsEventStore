using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace RdbmsEventStore.EventRegistry
{
    public class EventRegistryBuilder
    {
        private readonly IDictionary<string, Type> _registry;

        public EventRegistryBuilder() { _registry = new Dictionary<string, Type>(); }

        public Func<Type, string> DefaultNamer = type => type.AssemblyQualifiedName;

        public EventRegistryBuilder AddTypes(IEnumerable<Type> types)
            => AddTypes(types, DefaultNamer);

        public EventRegistryBuilder AddTypes(IEnumerable<Type> types, Func<Type, string> namer)
        {
            foreach (var type in types)
            {
                _registry.Add(namer(type), type);
            }
            return this;
        }

        public EventRegistryBuilder AddEventTypesFromAssembly<TEventBase>()
            => AddEventTypesFromAssembly<TEventBase>(typeof(TEventBase));

        public EventRegistryBuilder AddEventTypesFromAssembly<TEventBase>(Type assemblyMarkerType)
            => AddEventTypesFromAssembly<TEventBase>(assemblyMarkerType, DefaultNamer);

        public EventRegistryBuilder AddEventTypesFromAssembly<TEventBase>(Type assemblyMarkerType, Func<Type, string> namer)
            => AddTypes(assemblyMarkerType
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .Where(t => typeof(TEventBase).IsAssignableFrom(t)), namer);

        public EventRegistry Build()
            => new EventRegistry(_registry.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
    }
}