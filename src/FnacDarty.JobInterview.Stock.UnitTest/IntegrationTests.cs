using FluentAssertions;
using FnacDarty.JobInterview.Stock.Core.Entities;
using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class IntetgrationTests : TestBase
    {
        #region Fields

        #endregion

        [Test]
        public async Task Should_Create_Flow_With_One_Product()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            // Act
            await stockManager.CreateFlow(DateTime.Today, "Move 1", new Product { Ean = "ABCDEFGH" }, 10);


            // Assert
            Store.Flows.Values.Should().HaveCount(1);
        }

        public async Task Should_Create_Inventory_Flow_With_One_Product()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            // Act
            await stockManager.CreateInventoryFlow("Move 1", new Product { Ean = "ABCDEFGH" }, 10);

            // Assert
            Store.Flows.Values.Should().HaveCount(1);
        }

        [Test]
        public async Task Should_Create_Flow_With_Multiple_Lines()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            // Act
            await stockManager.CreateFlow(DateTime.Today, "Move 1", new List<ProductLine>
            {
                new ProductLine { Product = new Product { Ean ="ABCDEFGH" }, Quantity = 10 },
                new ProductLine { Product = new Product { Ean ="IJKLMNOP" }, Quantity = 20 }
            });

            // Assert
            Store.Flows.Values.Should().HaveCount(1);
        }

        [Test]
        public async Task Should_Create_Multiple_Flow_With_Multiple_Lines()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            var productLines1 = new List<ProductLine>
                {
                    new ProductLine { Product = new Product { Ean ="ABCDEFGH" }, Quantity = 10 },
                    new ProductLine { Product = new Product { Ean ="IJKLMNOP" }, Quantity = 20 }
                };

            var productLines2 = new List<ProductLine>
                {
                    new ProductLine { Product = new Product { Ean ="KLMNOPQR" }, Quantity = 10 },
                    new ProductLine { Product = new Product { Ean ="IJKLMNOP" }, Quantity = 20 }
                };
            ; var multi = new List<List<ProductLine>> { productLines1, productLines2 };

            // Act
            await stockManager.CreateMultipleFlow(DateTime.Today, "Move 1 and 2", multi);

            // Assert
            Store.Flows.Values.Should().HaveCount(2);
        }

        [Test]
        public async Task Should_Get_Stock_By_Date()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            var product = new Product { Ean = "A" };
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow {
                Date = DateTime.Today,
                Label = "Move1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product , Quantity = 10 }
                }
            }));
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today,
                Label = "Move1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product , Quantity = -5 }
                }
            }));


            // Act
            var result = await stockManager.GetStockByDate(DateTime.Today);

            // Assert
            result.Should().BeEquivalentTo(new[] { (product,5) });
        }

        [Test]
        public async Task Should_Get_Products_In_Stock()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            var product1 = new Product { Ean = "A" };
            var product2 = new Product { Ean = "B" };
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today,
                Label = "Move1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product1 , Quantity = 10 }
                }
            }));
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today,
                Label = "Move2",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product2 , Quantity = -5 }
                }
            }));


            // Act
            var result = await stockManager.GetProductsInStock(DateTime.Today);

            // Assert
            result.Should().BeEquivalentTo(new[] { (product1, 10) });
        }

        [Test]
        public async Task Should_Get_Count_Products_In_Stock()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            var product1 = new Product { Ean = "A" };
            var product2 = new Product { Ean = "B" };
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today,
                Label = "Move1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product1 , Quantity = 10 }
                }
            }));
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today,
                Label = "Move2",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product2 , Quantity = -5 }
                }
            }));


            // Act
            var result = await stockManager.GetProductCount(DateTime.Today);

            // Assert
            result.Should().Be(1);
        }

        [Test]
        public async Task Should_Get_Variation_By_Date()
        {
            // Arrange
            var stockManager = StockManager.Instance;
            Store.Flows = new ConcurrentDictionary<Guid, Lazy<Flow>>();
            Store.Products = new ConcurrentDictionary<string, Lazy<Product>>();

            var product = new Product { Ean = "A" };
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today.AddDays(-5),
                Label = "Move1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product , Quantity = 10 }
                }
            }));
            Store.Flows.TryAdd(Guid.NewGuid(), new Lazy<Flow>(() => new Flow
            {
                Date = DateTime.Today,
                Label = "Move1",
                IsRegularizationFlow = false,
                FlowLines = new List<ProductLine> {
                    new ProductLine { Product = product , Quantity = -5 }
                }
            }));


            // Act
            var result = await stockManager.GetStockVariation(DateTime.Today.AddDays(-4), DateTime.Today);

            // Assert
            result.Should().BeEquivalentTo(new[] { (product, -5) });
        }
    }
}
