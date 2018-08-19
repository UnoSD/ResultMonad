using System.Threading.Tasks;
using NUnit.Framework;
using Monad.TaskResult;
using static Result.ResultExtensions;

namespace Test
{
    [TestFixture]
    class ResultTaskTest
    {
        [Test]
        public void MonadResult()
        {
            var query =
                from i in "pippo".ToTask()
                from p in (i + "peppe").ToResult()
                select p;

            var chain =
                "pippo".ToTask()
                       .SelectMany(i => (i + "peppe").ToResult());

            Assert.That(query.Result(), Is.EqualTo(chain.Result()));
            Assert.That(chain.Result(), Is.EqualTo("pippopeppe"));
        }
    }
}