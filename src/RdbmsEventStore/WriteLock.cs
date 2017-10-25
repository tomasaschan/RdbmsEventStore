using System;
using System.Collections.Concurrent;
using Nito.AsyncEx;

namespace RdbmsEventStore
{
    public class WriteLock<TStreamId> : IWriteLock<TStreamId>
    {
        private readonly ConcurrentDictionary<TStreamId, AsyncLock> _mutexes = new ConcurrentDictionary<TStreamId, AsyncLock>();

        public AwaitableDisposable<IDisposable> Aquire(TStreamId streamId) => _mutexes.GetOrAdd(streamId, id => new AsyncLock()).LockAsync();
    }
}