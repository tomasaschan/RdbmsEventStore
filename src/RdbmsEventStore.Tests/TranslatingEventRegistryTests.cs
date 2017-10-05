using System.Collections.Generic;
using Moq;
using RdbmsEventStore.EventRegistry;
using Xunit;

namespace RdbmsEventStore.Tests
{
    public class TranslatingEventRegistryTests
    {
        private class Foo { }

        public class UnidirectionalTranslations
        {
            private readonly TranslatingEventRegistry _registry;

            public UnidirectionalTranslations()
            {
                var innerRegistry = new Mock<IComposableEventRegistry>(MockBehavior.Strict);
                innerRegistry.Setup(r => r.NameFor(typeof(Foo))).Returns(nameof(Foo));
                innerRegistry.Setup(r => r.TypeFor(nameof(Foo))).Returns(typeof(Foo));

                _registry = new TranslatingEventRegistry(new Dictionary<string, string>
                {
                    {"Bar", "Foo"}
                }, innerRegistry.Object);
            }

            [Fact]
            public void CanReturnTypesNotRegisteredInTranslations()
            {
                Assert.Equal(typeof(Foo), _registry.TypeFor(nameof(Foo)));
            }

            [Fact]
            public void CanReturnTypesRegisteredInTranslations()
            {
                Assert.Equal(typeof(Foo), _registry.TypeFor("Bar"));
            }

            [Fact]
            public void ReturnsNewName()
            {
                Assert.Equal(nameof(Foo), _registry.NameFor(typeof(Foo)));
            }
        }

        public class BidirectionalTranslations
        {
            private readonly TranslatingEventRegistry _registry;

            public BidirectionalTranslations()
            {
                var innerRegistry = new Mock<IComposableEventRegistry>(MockBehavior.Strict);
                innerRegistry.Setup(r => r.NameFor(typeof(Foo))).Returns(nameof(Foo));
                innerRegistry.Setup(r => r.TypeFor(nameof(Foo))).Returns(typeof(Foo));
                
                _registry = new TranslatingEventRegistry(new Dictionary<string, string>
                {
                    {"Bar", "Foo"}
                }, innerRegistry.Object, translateNewEvents: true);
            }

            [Fact]
            public void CanReturnTypesNotRegisteredInTranslations()
            {
                Assert.Equal(typeof(Foo), _registry.TypeFor(nameof(Foo)));
            }

            [Fact]
            public void CanReturnTypesRegisteredInTranslations()
            {
                Assert.Equal(typeof(Foo), _registry.TypeFor("Bar"));
            }

            [Fact]
            public void ReturnsOldName()
            {
                Assert.Equal("Bar", _registry.NameFor(typeof(Foo)));
            }
        }
    }
}