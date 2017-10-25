using System;
using Nito.AsyncEx;

namespace RdbmsEventStore
{
    public interface IWriteLock<in TStreamId>
    {
        AwaitableDisposable<IDisposable> Aquire(TStreamId streamId);
    }
}