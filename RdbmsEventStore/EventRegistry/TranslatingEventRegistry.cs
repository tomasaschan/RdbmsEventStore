using System;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class TranslatingEventRegistry : IEventRegistry
    {
        private readonly IReadOnlyDictionary<string, string> _translations;
        private readonly IReadOnlyDictionary<string, string> _inverseTranslations;
        private readonly IEventRegistry _registry;
        private readonly bool _translateNewEvents;

        public TranslatingEventRegistry(IReadOnlyDictionary<string, string> translations, IEventRegistry registry, bool translateNewEvents = false)
        {
            _translations = translations;
            if (translateNewEvents)
            {
                _inverseTranslations = translations.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
            }

            _registry = registry;
            _translateNewEvents = translateNewEvents;
        }

        public Type TypeFor(string eventType)
            => _translations.TryGetValue(eventType, out var translated)
                ? _registry.TypeFor(translated)
                : _registry.TypeFor(eventType);

        public string NameFor(Type eventType)
        {
            var registeredName = _registry.NameFor(eventType);
            return _translateNewEvents && _inverseTranslations.TryGetValue(registeredName, out var translated)
                ? translated
                : registeredName;
        }

        public IReadOnlyDictionary<string, Type> Registrations
            => _registry
                .Registrations
                .ToDictionary(
                    kvp => _translations.TryGetValue(kvp.Key, out var translated)
                        ? translated
                        : kvp.Key,
                    kvp => kvp.Value);
    }
}