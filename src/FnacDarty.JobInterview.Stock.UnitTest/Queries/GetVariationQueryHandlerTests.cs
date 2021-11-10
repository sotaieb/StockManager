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
using System.Linq;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class GetVariationQueryHandlerTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(GetVariationQueryHandler).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Should_Return_Stock_Variation(
            [Frozen] Mock<IStockService> stockServiceMock,
            GetVariationQueryHandler sut)
        {
            // Arrange
            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var stock1 = new[] { (product1, 10), (product2, 5) };
            var stock2 = new[] { (product1, 20), (product2, -5) };

            stockServiceMock.Setup(x => x.GetStockByDate(DateTime.Today.Date.AddDays(-1)))
                            .ReturnsAsync(stock1);
            stockServiceMock.Setup(x => x.GetStockByDate(DateTime.Today.Date))
                           .ReturnsAsync(stock2);

            var request = new GetVariationQuery { 
                StartDate = DateTime.Today.Date.AddDays(-1),
                EndDate = DateTime.Today.Date
            };

            // Act
            var result = await sut.Handle(request, Source.Token);

            // Assert           
            Assert.That(() => result.Count() == 2);
            result.Should().BeEquivalentTo(new[] { (product1, 10), (product2, -10) });

            await Task.CompletedTask;
        }
    }
}
