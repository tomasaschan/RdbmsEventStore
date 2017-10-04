using System;

namespace RdbmsEventStore.EntityFramework.Tests.TestData
{
    public class TestEvent : Event<Guid, Guid> { }
}