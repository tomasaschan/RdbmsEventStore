using System;
using System.Linq;
using System.Reflection;

namespace RdbmsEventStore.EventRegistry
{
    public class AssemblyEventRegistry : SimpleEventRegistry
    {
        public AssemblyEventRegistry(Type markerType)
            : this(markerType, type => type.Name, type => true)
        {
        }

        public AssemblyEventRegistry(Type markerType, Func<Type, bool> inclusionPredicate)
            : this(markerType, type => type.Name, inclusionPredicate)
        {
        }

        public AssemblyEventRegistry(Type markerType, Func<Type, string> namer)
            : this(markerType, namer, type => true)
        {
        }

        public AssemblyEventRegistry(Type markerType, Func<Type, string> namer, Func<Type, bool> inclusionPredicate)
            : base(markerType
                .GetTypeInfo()
                .Assembly
                .GetTypes()
                .Where(type => !type.Name.StartsWith("<>"))
                .Where(inclusionPredicate), namer)
        {
        }
    }
}