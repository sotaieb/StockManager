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
using System;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class GetCountProductsInStockQueryHandlerTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(GetCountProductsInStockQueryHandler).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task With_Date_Returns_Count_Products_In_Stock(
            [Frozen] Mock<IStockService> stockServiceMock,
            GetCountProductsInStockQueryHandler sut)
        {
            // Arrange

            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };
            var product3 = new Product { Ean = "QRSTUVWZ" };
            var stock = new[] { (product1, 10), (product2, -1), (product3,50) };

            stockServiceMock.Setup(x => x.GetStockByDate(DateTime.Today.AddDays(-1)))
                .ReturnsAsync(stock);

            var request = new GetCountProductsInStockQuery { Date = DateTime.Today.Date.AddDays(-1) };

            // Act
            var result = await sut.Handle(request, Source.Token);

            // Assert
            
            result.Should().Be(2);

            await Task.CompletedTask;
        }

        
    }
}
