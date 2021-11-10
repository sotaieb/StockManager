using FluentAssertions;
using FnacDarty.JobInterview.Stock.Core.Entities;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class FunctionalTests : TestBase
    {
        #region Fields
        private StockManager _stockManager = StockManager.Instance;

        private static Random _random = new Random();

        #endregion

        [Test]
        public async Task Concurrency_Senario_Test()
        {
            // Arrange
            var products = GenerateMultipleProducts();
            var flows = GenerateMultipleFlows(products);
            
            var localFlows = new ThreadLocal<Flow[]>(() => flows);
            var tasks = new Task[10];

            // Act
            for (int i = 0; i < 10; i++)
            {
                object o = i;
                tasks[i] = Task.Run(async () =>
                {
                    var k = (int)o;

                    await _stockManager.CreateFlow(
                        localFlows.Value[k].Date,
                        localFlows.Value[k].Label,
                        localFlows.Value[k].FlowLines);
                });
            }

            try
            {
                //Task.WaitAll(tasks);
                await Task.WhenAll(tasks);

                var stock = await _stockManager.GetStockByDate();
                
                // Assert
                stock.Should().BeEquivalentTo(products.Select(x => (x, 0)));

                foreach (var item in stock)
                {
                    TestContext.WriteLine($"{item.Item1.Ean}-{item.Item2}");
                }
                // Console.WriteLine(JsonConvert.SerializeObject(products, Formatting.Indented));
                //Console.WriteLine(JsonConvert.SerializeObject(_flows.Value, Formatting.Indented));
                //Console.WriteLine(JsonConvert.SerializeObject(Store.Flows.Values, Formatting.Indented));

            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        #region Helpers
        private static Flow[] GenerateMultipleFlows(Product[] products)
        {
            var flows = new Flow[10];

            for (int i = 0; i < 10; i++)
            {
                var lines = new List<(Product, int)>();
                for (int j = 0; j < 10; j++)
                {
                    lines.Add((products[j], i % 2 == 0 ? 10 : -10));

                }
                var flow = GenerateFlow(i % 2 == 0, lines.ToArray());

                flows[i] = flow;
            }

            return flows;
        }
        private static Product[] GenerateMultipleProducts()
        {
            var products = new Product[10];
            for (int i = 0; i < 10; i++)
            {
                products[i] = GenerateProduct();
            }
            return products;
        }
        public static Flow GenerateFlow(bool isRegulationFlow = false, params (Product, int)[] products)
        {
            var r = new Random();
            int i = r.Next(0, 100);
            var flow = new Flow
            {

                Date = DateTime.Today,
                Label = $"Move {Guid.NewGuid()}",
                IsRegularizationFlow = isRegulationFlow,
                FlowLines = new List<ProductLine>()
            };
            foreach (var item in products)
            {
                flow.FlowLines.Add(new ProductLine
                {
                    Product = item.Item1,
                    Quantity = item.Item2
                });
            }
            return flow;
        }
        public static Product GenerateProduct()
        {
            return new Product { Ean = RandomString(8) };
        }
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        #endregion
    }
}
