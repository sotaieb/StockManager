using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Queries;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.UnitTest.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class GetStockQueryHandlerTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(GetStockQueryHandler).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task Should_Return_Last_Stock_State(
            [Frozen] Mock<IStockService> stockServiceMock,
            GetStockQueryHandler sut)
        {
            // Arrange
            var stock = new[] { (new Product { Ean = "ABCDEFGH" }, 10), (new Product { Ean = "IJKLMNOP" }, 5) };
            stockServiceMock.Setup(x => x.GetStockByDate(null))
                .ReturnsAsync(stock);

            var request = new GetStockQuery();
            // Act
            var result = await sut.Handle(request, Source.Token);

            // Assert
            Assert.That(() => result.Count() == 2);
            result.Should().BeEquivalentTo(stock);

            await Task.CompletedTask;
        }
    }
}
