using System.Linq;
using System.Threading.Tasks;
using static Result.ResultExtensions;
using NUnit.Framework;
using EnumerableResult;

namespace Test
{
    [TestFixture]
    class EnumerableResultTest
    {
        [Test]
        public void Monad()
        {
            var query =
                from i in new[] { "pippo" }.AsEnumerable().ToResult()
                from p in (i + "peppe").ToResult()
                select p;
            
            var chain =
                new[] { "pippo" }.AsEnumerable()
                                 .ToResult()
                                 .SelectMany(i => (i + "peppe").ToResult());

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
        }

        [Test]
        public void MonadProjection()
        {
            var query =
                from i in new[] { "pippo" }.AsEnumerable().ToResult()
                from p in (i + "peppe").ToResult()
                select p + i;
            
            var chain =
                new[] { "pippo" }.AsEnumerable()
                                 .ToResult()
                                 .SelectMany(i => (i + "peppe").ToResult(),
                                             (i, p) => p + i);

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppepippo"));
        }

        [Test]
        public void MonadTask()
        {
            var query =
                from i in new[] { "pippo" }.AsEnumerable().ToResult()
                from p in (i + "peppe").ToResult().ToTask()
                select p;
            
            var chain =
                new[] { "pippo" }.AsEnumerable()
                                 .ToResult()
                                 .SelectMany(i => (i + "peppe").ToResult().ToTask());

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
        }
    }
}