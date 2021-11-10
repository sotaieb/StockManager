using NUnit.Framework;
using System.Threading;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class TestBase
    {
        public CancellationTokenSource Source { get; set; }
        private TestContext _testContext;

        public TestContext TestContext
        {
            get { return _testContext; }
            set { _testContext = value; }
        }

        [SetUp]
        public void SetUp()
        {
            Source = new CancellationTokenSource();
        }

        [TearDown]
        public void TearDown()
        {
        }
    }
}
