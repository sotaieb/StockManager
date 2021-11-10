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
    public class GetVariationByProductQueryHandlerTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(GetVariationByProductQueryHandler).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Should_Return_Stock_Variation_Of_Product(
            [Frozen] Mock<IStockService> stockServiceMock,
            GetVariationByProductQueryHandler sut)
        {
            // Arrange
            var product1 = new Product { Ean = "ABCDEFGH" };
            var stock1 = (product1, 10);
            var stock2 = (product1, -10);

            stockServiceMock.Setup(x => x.GetStockByProductAndDate(product1, DateTime.Today.Date.AddDays(-1)))
                            .ReturnsAsync(stock1);
            stockServiceMock.Setup(x => x.GetStockByProductAndDate(product1, DateTime.Today.Date))
                           .ReturnsAsync(stock2);

            var request = new GetVariationByProductQuery
            {
                Product = product1,
                StartDate = DateTime.Today.Date.AddDays(-1),
                EndDate = DateTime.Today.Date
            };

            // Act
            var result = await sut.Handle(request, Source.Token);

            // Assert           
           
            result.Should().BeEquivalentTo((product1, -20));

            await Task.CompletedTask;
        }
    }
}
