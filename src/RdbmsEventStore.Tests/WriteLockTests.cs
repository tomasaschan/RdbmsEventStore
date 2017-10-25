using Xunit;

namespace RdbmsEventStore.Tests
{
    public class WriteLockTests
    {
        [Fact]
        public void SameIdCannotEnterLockSimultaneously()
        {
            var mutex = new WriteLock<int>();

            var lock1 = mutex.Aquire(1);
            var lock2 = mutex.Aquire(1);

            Assert.True(lock1.AsTask().IsCompleted);
            Assert.False(lock2.AsTask().IsCompleted);
        }

        [Fact]
        public void DifferentIdsCanEnterLockSimultaneously()
        {
            var mutex = new WriteLock<int>();

            var lock1 = mutex.Aquire(1);
            var lock2 = mutex.Aquire(2);

            Assert.True(lock1.AsTask().IsCompleted, "First stream could not enter locked path.");
            Assert.True(lock2.AsTask().IsCompleted, "Second stream could not enter locked path.");
        }
    }
}