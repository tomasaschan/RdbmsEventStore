using System;
using System.Collections.Generic;

namespace RdbmsEventStore
{
    public interface IMaterializer
    {
        TState Unfold<TState, TId, TStreamId>(TState initialState, IEnumerable<IEvent<TId, TStreamId>> events, Func<TState, object, TState> applicator);
    }
}