using System;
using System.Linq;
using Xunit;

namespace tlycken.Extensions.Tests
{
    public class QueryableExtensions
    {
        public class Foo
        {
            public bool Bar { get; set; }
        }

        [Fact]
        public void CanApplyProjectionToQueryables()
        {
            var foos = new[] { new Foo { Bar = true }, new Foo { Bar = false }, new Foo { Bar = true } }.AsQueryable();

            var result = foos.Apply(fs => fs.Where(foo => foo.Bar));

            Assert.Equal(2, result.Count());
        }
    }
}
