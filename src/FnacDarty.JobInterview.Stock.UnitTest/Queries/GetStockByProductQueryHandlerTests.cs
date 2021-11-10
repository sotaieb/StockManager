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
    public class GetStockByProductQueryHandlerTests: TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(GetStockByProductQueryHandler).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Should_Return_Last_Stock_Of_Product(
            [Frozen] Mock<IStockService> stockServiceMock,
            GetStockByProductQueryHandler sut)
        {
            // Arrange
            var product = new Product { Ean = "ABCDEFGH" };
           
            stockServiceMock.Setup(x => x.GetStockByProductAndDate(product, DateTime.Today))
                .ReturnsAsync((product, 10));

            var request = new GetStockByProductQuery { 
                Date=DateTime.Today, Product = product 
            };
            // Act
            var result = await sut.Handle(request, Source.Token);

            // Assert
           
            result.Should().BeEquivalentTo((new Product { Ean = "ABCDEFGH" }, 10));

            await Task.CompletedTask;
        }
    }
}
