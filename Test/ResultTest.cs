using Result;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class ResultTest
    {
        [Test]
        public void Functor()
        {
            var query =
                from i in "pippo".ToResult()
                select i + "peppe";

            var chain =
                "pippo".ToResult()
                       .Select(i => i + "peppe");

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
        }

        [Test]
        public void Monad()
        {
            var query =
                from i in "pippo".ToResult()
                from p in (i + "peppe").ToResult()
                select p;

            var chain =
                "pippo".ToResult()
                       .SelectMany(i => (i + "peppe").ToResult());

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
        }

        [Test]
        public void MonadProjection()
        {
            var query =
                from i in "pippo".ToResult()
                from p in (i + "peppe").ToResult()
                select p + i;

            var chain =
                "pippo".ToResult()
                       .SelectMany(i => (i + "peppe").ToResult(),
                                   (i, p) => p + i);

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppepippo"));
        }

        [Test]
        public void MonadMultiple()
        {
            var query =
                from i in "pippo".ToResult()
                from p in (i + "peppe").ToResult()
                from q in (p + "peppa").ToResult()
                from r in (q + "pippi").ToResult()
                select r;

            var chain =
                "pippo".ToResult()
                       .SelectMany(i => (i + "peppe").ToResult())
                       .SelectMany(p => (p + "peppa").ToResult())
                       .SelectMany(q => (q + "pippi").ToResult());

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppepeppapippi"));
        }
    }
}
