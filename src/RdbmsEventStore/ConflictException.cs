using System;
using System.Linq;

namespace RdbmsEventStore
{
    public class ConflictException : InvalidOperationException
    {
        public ConflictException(object streamId, DateTimeOffset? aggregateVersionBefore, DateTimeOffset? currentVersion, params object[] payloads)
            : base(@"Event store conflict: You are trying to commit events which are based on an outdated view of the state. " + 
                  "Re-load the state, re-apply your actions based on the up-to-date information, and try again.\n" +
                   $"Event stream: {streamId}\nVersion in memory: {aggregateVersionBefore?.ToString() ?? "<initial>"}\nVersion in database: {currentVersion?.ToString() ?? "<initial>"}\n" +
                  $"Events: {string.Join(", ", payloads.Select(p => p.GetType().Name))}")
        {
        }
    }
}