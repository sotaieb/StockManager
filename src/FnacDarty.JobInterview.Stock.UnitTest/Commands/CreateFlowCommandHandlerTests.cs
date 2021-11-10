using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Idioms;
using AutoFixture.NUnit3;
using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Commands;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Infrastructure.Exceptions;
using FnacDarty.JobInterview.Stock.UnitTest.Helpers;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    
    public class CreateFlowCommandHandlerTests : TestBase
    {
        [Test]
        public static void Ensure_Constructor_Arguments_NotNull()
        {
            // Arrange
            var fixture = new Fixture().Customize(new AutoMoqCustomization());
            var assertion = new GuardClauseAssertion(fixture);

            // Act, Assert
            assertion.Verify(typeof(CreateFlowCommandHandler).GetConstructors());
        }

        [TestCaseSource(typeof(CreateFlowCommandDataSource), nameof(CreateFlowCommandDataSource.InvalidFlows))]
        public async Task When_Invalid_Request_Throws_Request_Exception(CreateFlowCommand request)
        {
            // Arrange
            var productService = Mock.Of<IProductService>();
            var flowService = Mock.Of<IFlowService>();
                        
            var handler = new CreateFlowCommandHandler(
                productService, flowService);
            // Act

            // Assert
            Assert.ThrowsAsync<RequestException>(
                async () => { await handler.Handle(request, Source.Token); });

            await Task.CompletedTask;
        }

        [Test, AutoMoqData]
        public async Task When_New_Product_Should_Create_Product_Then_Create_Flow(            
            [Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            CreateFlowCommandHandler sut
            )
        {
            // Arrange
            var product = new Product { Ean = "ABCDEFGH" };
            var request = new CreateFlowCommand
            {
                Date = DateTime.Now,
                Label = "Move 1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine {
                        Product = product,
                        Quantity = 10
                    }
                }
            };
                  

            // Act
            await sut.Handle(request, Source.Token);

            // Assert
            productServiceMock.Verify(x => x.AddProduct(product), Times.Once);
            flowServiceMock.Verify(x => x.AddFlow(It.IsAny<Flow>(), It.IsAny<CancellationToken>()), Times.Once);
            await Task.CompletedTask;
        }

        [Test, AutoMoqData]
        public async Task When_Date_Before_Regularization_Throws_Exception(
            [Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            CreateFlowCommandHandler sut)
        {
            // Arrange            
            var date = DateTime.Now.AddDays(-5);
            var product = new Product { Ean = "ABCDEFGH" };
            flowServiceMock.Setup(x => x.GetLastInventoryDate(product, It.IsAny<CancellationToken>()))
                .ReturnsAsync(DateTime.Today);

            var request = new CreateFlowCommand
            {
                Date = date,
                Label = "Move 1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine {
                        Product = product,
                        Quantity = 10
                    }
                }
            };
              
            // Act

            // Assert
            Assert.ThrowsAsync<RequestException>(
               async () => { await sut.Handle(request, Source.Token); });

            productServiceMock.Verify(x => x.AddProduct(product), Times.Never);
            flowServiceMock.Verify(x => x.AddFlow(It.IsAny<Flow>(), It.IsAny<CancellationToken>()), Times.Never);

            await Task.CompletedTask;
        }
               
        [Test, AutoMoqData]
        public async Task When_Multiple_Product_Should_Validate_And_Create_Flow(
             [Frozen] Mock<IProductService> productServiceMock,
            [Frozen] Mock<IFlowService> flowServiceMock,
            CreateFlowCommandHandler sut)
        {
            // Arrange         
            var date = DateTime.Now;
            var product1 = new Product { Ean = "ABCDEFGH" };
            var product2 = new Product { Ean = "IJKLMNOP" };

            var request = new CreateFlowCommand
            {
                Date = date,
                Label = "Move 1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine {
                        Product = product1,
                        Quantity = 10
                    },
                    new ProductLine {
                        Product = product2,
                        Quantity = 20
                    },
                }
            };
           
            // Act
            await sut.Handle(request, Source.Token);

            // Assert
            productServiceMock.Verify(x => x.AddProduct(product1), Times.Once);
            productServiceMock.Verify(x => x.AddProduct(product2), Times.Once);
            flowServiceMock.Verify(x => x.AddFlow(It.IsAny<Flow>(), It.IsAny<CancellationToken>()), Times.Once);
            await Task.CompletedTask;
        }
    }
}
