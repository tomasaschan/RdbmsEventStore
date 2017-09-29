using System;
using Nito.AsyncEx;

namespace RdbmsEventStore
{
    public class WriteLock : IWriteLock
    {
        private readonly AsyncLock _mutex = new AsyncLock();

        public AwaitableDisposable<IDisposable> Aquire() => _mutex.LockAsync();
    }
}