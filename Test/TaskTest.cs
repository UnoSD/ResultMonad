using System.Threading.Tasks;
using NUnit.Framework;

namespace Test
{
    [TestFixture]
    class TaskTest
    {
        [Test]
        public void Functor()
        {
            var query =
                from i in "pippo".ToTask()
                select i + "peppe";

            var chain =
                "pippo".ToTask()
                       .Select(i => i + "peppe");

            Assert.That(query.Result, Is.EqualTo(chain.Result));
            Assert.That(chain.Result, Is.EqualTo("pippopeppe"));
        }

        [Test]
        public void Monad()
        {
            var query =
                from i in "pippo".ToTask()
                from p in (i + "peppe").ToTask()
                select p;

            var chain =
                "pippo".ToTask()
                       .SelectMany(i => (i + "peppe").ToTask());

            Assert.That(query.Result, Is.EqualTo(chain.Result));
            Assert.That(chain.Result, Is.EqualTo("pippopeppe"));
        }
    }
}