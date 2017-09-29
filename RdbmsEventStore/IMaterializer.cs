using System;
using System.Collections.Generic;

namespace RdbmsEventStore
{
    public interface IMaterializer
    {
        TState Unfold<TState, TId>(TState initialState, IEnumerable<IEvent<TId>> events, Func<TState, object, TState> applicator);
    }
}