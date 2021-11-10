using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Queries;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.UnitTest.Helpers;
using Moq;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class GetProductsInStockQueryHandlerTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(GetProductsInStockQueryHandler).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Should_Return_Current_Products_In_Stock(
            [Frozen] Mock<IStockService> stockServiceMock,
            GetProductsInStockQueryHandler sut)
        {
            // Arrange

            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var stock = new[] { (product1, 10), ( product2, -1) };
            stockServiceMock.Setup(x => x.GetStockByDate(null))
                .ReturnsAsync(stock);

            var request = new GetProductsInStockQuery();
            
            // Act
            var result = await sut.Handle(request, Source.Token);

            // Assert
            Assert.That(() => result.Count() == 1);
            result.Should().BeEquivalentTo(new[] { (product1, 10) });

            await Task.CompletedTask;
        }
    }
}
