using System;

namespace RdbmsEventStore.EntityFramework.Tests.TestData
{
    public class GuidGuidEvent : Event<Guid, Guid> { }
    public class LongStringEvent : Event<long, string> { }
    public class LongLongEvent : Event<long, long> { }

    public class FooEvent
    {
        public string Foo { get; set; }
    }

    public class BarEvent
    {
        public string Bar { get; set; }
    }
}