using System.Threading.Tasks;
using Result;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class TaskResultTest
    {
        [Test]
        public void Functor()
        {
            var query =
                from t in "pippo".ToResult()
                select Task.FromResult(t);

            var chain =
                "pippo".ToResult()
                       .Select(Task.FromResult);

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippo"));
        }

        [Test]
        public void Monad()
        {
            var query =
                from i in "pippo".ToResult().ToTask()
                from p in (i + "peppe").ToResult()
                select p;

            var chain =
                "pippo".ToResult()
                       .ToTask()
                       .SelectMany(i => (i + "peppe").ToResult());

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
        }

        [Test]
        public void MonadProjection()
        {
            var query =
                from i in "pippo".ToResult().ToTask()
                from p in (i + "peppe").ToResult()
                select p + i;

            var chain =
                "pippo".ToResult()
                       .ToTask()
                       .SelectMany(i => (i + "peppe").ToResult(),
                                   (i, p) => p + i);

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppepippo"));
        }
    }
}