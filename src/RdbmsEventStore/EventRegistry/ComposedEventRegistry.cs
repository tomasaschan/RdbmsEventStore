using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class ComposedEventRegistry : SimpleEventRegistry
    {
        public ComposedEventRegistry(params IComposableEventRegistry[] registries)
            : base(registries.SelectMany(r => r.Registrations))
        {
        }
    }
}