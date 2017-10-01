using System;
using Nito.AsyncEx;

namespace RdbmsEventStore
{
    public interface IWriteLock
    {
        AwaitableDisposable<IDisposable> Aquire();
    }
}