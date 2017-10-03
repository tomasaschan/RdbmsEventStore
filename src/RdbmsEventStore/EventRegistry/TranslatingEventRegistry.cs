using System;
using System.Collections.Generic;
using System.Linq;

namespace RdbmsEventStore.EventRegistry
{
    public class TranslatingEventRegistry : IComposableEventRegistry
    {
        private readonly IReadOnlyDictionary<string, string> _translations;
        private readonly IReadOnlyDictionary<string, string> _inverseTranslations;
        private readonly IComposableEventRegistry _inner;
        private readonly bool _translateNewEvents;

        public TranslatingEventRegistry(IReadOnlyDictionary<string, string> translations, IComposableEventRegistry inner, bool translateNewEvents = false)
        {
            _translations = translations;
            if (translateNewEvents)
            {
                try
                {
                    _inverseTranslations = translations.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
                }
                catch (ArgumentException ex)
                {
                    throw new EventTypeRegistrationException(
                        "Invalid translation registration; two or more old names translate to the same new name.\n" +
                        "If you want to translate both Foo and Bar to Baz, add Foo=>Bar and Bar=>Baz instead of Foo=>Baz and Bar=>Baz.", ex);
                }
            }

            _inner = inner;
            _translateNewEvents = translateNewEvents;
        }

        public Type TypeFor(string eventType)
            => _inner.TypeFor(TranslationFor(eventType));

        public string NameFor(Type eventType)
            => InverseTranslationFor(_inner.NameFor(eventType));

        private string TranslationFor(string eventType)
            => _translations.TryGetValue(eventType, out var translated)
                ? TranslationFor(translated)
                : eventType;

        private string InverseTranslationFor(string eventType)
            => _inverseTranslations.TryGetValue(eventType, out var translated)
                ? InverseTranslationFor(translated)
                : eventType;

        public IReadOnlyDictionary<string, Type> Registrations
            => _inner
                .Registrations
                .ToDictionary(
                    kvp => TranslationFor(kvp.Key),
                    kvp => kvp.Value);
    }
}