using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

namespace FnacDarty.JobInterview.Stock.UnitTest.Helpers
{
    public class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute()
            : base(() => new Fixture().Customize(new AutoMoqCustomization()))
        {
        }
    }
}
