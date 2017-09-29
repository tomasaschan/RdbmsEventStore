using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.Linq;

namespace RdbmsEventStore
{
    public static class QueryableExtensions
    {
        public static TState Unfold<TState, TId>(this IEnumerable<IEvent<TId>> events, TState initialState, Func<TState, object, TState> applicator)
            => events
                .Select(e => (type: e.Type, payload: e.Payload))
                .Select(e => JsonConvert.DeserializeObject(Encoding.UTF8.GetString(e.payload), Type.GetType(e.type)))
                .Aggregate(initialState, applicator);
    }
}