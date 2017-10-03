using System.Data.Common;
using System.Data.Entity.Infrastructure;

namespace RdbmsEventStore.EntityFramework.Tests.Infrastructure
{
    public class EffortProviderFactory : IDbConnectionFactory
    {
        private static DbConnection _connection;
        private static readonly object _lock = new object();

        public static void ResetDb()
        {
            lock (_lock)
            {
                _connection = null;
            }
        }

        public DbConnection CreateConnection(string nameOrConnectionString)
        {
            lock (_lock)
            {
                return _connection ?? (_connection = Effort.DbConnectionFactory.CreateTransient());
            }
        }
    }
}