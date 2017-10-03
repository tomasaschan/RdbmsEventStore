using System;
using RdbmsEventStore.EventRegistry;
using Xunit;

namespace RdbmsEventStore.Tests
{
    public class AssemblyEventRegistryTests
    {
        // Create two types with lambdas in them, to test handling of closure types
        private class FooContainer { private Func<object, bool> Foo = _ => true; }
        private class BarContainer { private Func<object, bool> Bar = _ => false; }

        [Fact]
        public void AssemblyRegistryCanHandleLambdaClosureTypes()
        {
            var _ = new AssemblyEventRegistry(typeof(AssemblyEventRegistryTests));
        }

        [Fact]
        public void AssemblyRegistryThrowsNiceExceptionsWithSameNamedTypes()
        {
            // use a really stupid namer, to create naming conflicts
            string StupidNamer(Type type) => type.Name.Substring(0, 1);

            Assert.Throws<EventTypeRegistrationException>(() => new AssemblyEventRegistry(typeof(AssemblyEventRegistryTests), (Func<Type, string>)StupidNamer));
        }
    }
}