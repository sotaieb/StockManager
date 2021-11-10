using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FluentAssertions;
using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.Infrastructure.Exceptions;
using FnacDarty.JobInterview.Stock.UnitTest.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class StockServiceTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(StockService).GetConstructors());
        }

        [Test, AutoMoqData]
        public async Task GetStockByDate_When_Default_Date_Should_Return_Last_Stock_State([Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            StockService sut)
        {
            // Arrange
            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var flows = new List<Flow>
            {
                new Flow { Date = DateTime.Today.AddDays(-1), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                        new ProductLine { Product = product2, Quantity = -20 }
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-2), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                    }
                }
            };

            flowServiceMock.Setup(x => x.GetFlowsFromStore())
                .ReturnsAsync(flows);

            // Act
            var result = await sut.GetStockByDate();

            // Assert

            result.Should()
                  .NotBeEmpty()
                  .And.HaveCount(2)
                  .And.ContainInOrder(new[] { (product1, 20), (product2, -20) });

            await Task.CompletedTask;
        }

        [Test, AutoMoqData]
        public async Task GetStockByDate_Given_Date_Should_Return_Stock_At_Date([Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            StockService sut)
        {
            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };
            // Arrange           
            var flows = new List<Flow>
            {
                new Flow { Date = DateTime.Today.AddDays(-1), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                        new ProductLine { Product = product2, Quantity = -20 }
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-10), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                    }
                }
            };
            flowServiceMock.Setup(x => x.GetFlowsFromStore())
                .ReturnsAsync(flows);

            // Act
            var result = await sut.GetStockByDate(DateTime.Today.AddDays(-5));

            // Assert

            result.Should()
                  .NotBeEmpty()
                  .And.HaveCount(1)
                  .And.ContainInOrder(new[] { (product1, 10) });

            await Task.CompletedTask;
        }

        [Test, AutoMoqData]
        public async Task GetStockByDate_When_Regularization_Flow_Should_Return_Stock_Correctly([Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            StockService sut)
        {
            // Arrange

            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var flows = new List<Flow>
            {
                new Flow { Date = DateTime.Today.AddDays(-1), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                        new ProductLine { Product = product2, Quantity = -20 }
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-2), Label="Move1", IsRegularizationFlow= true,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = -20 },
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-2), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 5 },
                    }
                }
            };
            flowServiceMock.Setup(x => x.GetFlowsFromStore())
                .ReturnsAsync(flows);

            // Act
            var result = await sut.GetStockByDate();

            // Assert

            result.Should()
                  .NotBeEmpty()
                  .And.HaveCount(2)
                  .And.ContainInOrder(new[] { (product1, -5), (product2, -20) });

            await Task.CompletedTask;
        }
        ///////
        [Test, AutoMoqData]
        public async Task GetStockByProduct_When_Null_Product_Throws_Request_Exception(
            StockService sut)
        {
            // Arrange

            // Act
            // Assert
            Assert.ThrowsAsync<RequestException>(
                async () => { await sut.GetStockByProductAndDate(null); });

            await Task.CompletedTask;

        }
        [Test, AutoMoqData]
        public async Task GetStockByProduct_When_Invalid_Product_Identifier_Throws_Request_Exception(
           StockService sut)
        {
            // Arrange

            // Act
            // Assert
            Assert.ThrowsAsync<RequestException>(
                async () => { await sut.GetStockByProductAndDate(new Product { Ean = "" }); });

            await Task.CompletedTask;

        }


        [Test, AutoMoqData]
        public async Task GetStockByProduct_When_Default_Date_Should_Return_Stock_State_Of_Product(
            [Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            StockService sut)
        {
            // Arrange
            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var flows = new List<Flow>
            {
                new Flow { Date = DateTime.Today.AddDays(-1), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                        new ProductLine { Product = product2, Quantity = -20 }
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-2), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                    }
                }
            };

            flowServiceMock.Setup(x => x.GetFlowsFromStore())
                .ReturnsAsync(flows);

            // Act
            var result = await sut.GetStockByProductAndDate(product1);

            // Assert

            result.Should()
                  .Be((product1, 20));

            await Task.CompletedTask;
        }

        [Test, AutoMoqData]
        public async Task GetStockByProduct_When_Anterior_Date_Should_Return_Stock_State_Of_Product_At_Date(
            [Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            StockService sut)
        {
            // Arrange
            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };
            var flows = new List<Flow>
            {
                new Flow { Date = DateTime.Today.AddDays(-1), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                        new ProductLine { Product = product2, Quantity = -20 }
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-10), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                    }
                }
            };
            flowServiceMock.Setup(x => x.GetFlowsFromStore())
                .ReturnsAsync(flows);

            // Act
            var result = await sut.GetStockByProductAndDate(product1, DateTime.Now.AddDays(-5));

            // Assert

            result.Should()
                  .Be((product1, 10));

            await Task.CompletedTask;
        }

        [Test, AutoMoqData]
        public async Task GetStockByProduct_When_Regularization_Should_Return_Stock_State_Of_Product_At_Date([Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            StockService sut)
        {
            // Arrange

            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var flows = new List<Flow>
            {
                new Flow { Date = DateTime.Today.AddDays(-10), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 10 },
                        new ProductLine { Product = product2, Quantity = -20 }
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-5), Label="Move1", IsRegularizationFlow= true,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = -20 },
                    }
                },
                 new Flow { Date = DateTime.Today.AddDays(-2), Label="Move1", IsRegularizationFlow= false,
                    FlowLines= new List<ProductLine>{
                        new ProductLine { Product = product1, Quantity = 5 },
                    }
                }
            };
            flowServiceMock.Setup(x => x.GetFlowsFromStore())
                .ReturnsAsync(flows);

            // Act
            var result = await sut.GetStockByProductAndDate(product1, DateTime.Now.AddDays(-1));

            // Assert

            result.Should()
                  .Be((product1, -15));

            await Task.CompletedTask;
        }
    }
}
