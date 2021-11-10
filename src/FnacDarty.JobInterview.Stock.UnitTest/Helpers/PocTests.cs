using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.UnitTest
{
    [TestFixture]
    public class PocTests
    {
        [Test]
        public void Variance_Test()
        {
            // Arrange
            var start = new List<(string, int)> {
                {("A",10) },
                {("B",20) },
                {("C",30) },
                {("D",50) },
            };
            var end = new List<(string, int)> {
                {("A",20) },
                {("B",5) },
                {("C",-10) },
                {("E",10) },
            };
            // Act

            var products = start.Select(x => x.Item1).Union(end.Select(x => x.Item1));
            var query = from id in products
                        join e in end on id equals e.Item1 into g1
                        from j1 in g1.DefaultIfEmpty((id, 0))
                        join s in start on j1.Item1 equals s.Item1 into g2
                        from j2 in g2.DefaultIfEmpty((id, 0))
                        select new { Product = id, Variance = j1.Item2 - j2.Item2 };

            var result = query.ToList();

            // Assert
            Assert.AreEqual(10, result.FirstOrDefault(x => x.Product == "A")?.Variance);
            Assert.AreEqual(-15, result.FirstOrDefault(x => x.Product == "B")?.Variance);
            Assert.AreEqual(-40, result.FirstOrDefault(x => x.Product == "C")?.Variance);
            Assert.AreEqual(-50, result.FirstOrDefault(x => x.Product == "D")?.Variance);
            Assert.AreEqual(10, result.FirstOrDefault(x => x.Product == "E")?.Variance);
            //foreach (var item in result)
            //{
            //    System.Console.WriteLine($"{item.Product} - {item.Variance}");
            //}

        }

        [Test]
        public void Grouping_Test()
        {
            // Arrange
            var start = new List<(DateTime, string, int, bool)> {
                {(DateTime.Now.AddDays(1), "A",10, false) },
                {(DateTime.Now.AddDays(2), "A",20, true) },
                {(DateTime.Now.AddDays(3), "B",30, false) },
                {(DateTime.Now.AddDays(4), "C",40, true) },
                {(DateTime.Now.AddDays(5), "A",50, true) }, //*
                {(DateTime.Now.AddDays(6), "A",60, false) },
            };

            // Act


            //var query = from s in start
            //            where s.Item4 == true
            //            group s by s.Item2 into g
            //            orderby g.Key
            //            select new
            //            {
            //                Product = g.Key,
            //                Last = g.OrderByDescending(x => x.Item1).Select(x =>
            //                        new
            //                        {
            //                            Date = x.Item1,
            //                            Quantity = x.Item3
            //                        }
            //                    )
            //                    .FirstOrDefault(),
            //            };


            var query = from s in start
                            //where s.Item2 == "H"
                        group s by s.Item2 into g
                        select new
                        {
                            Product = g.Key,
                            Lines = g.Where(x => x.Item1 >= g.Where(x => x.Item4)
                                                             .OrderByDescending(x => x.Item1)
                                                             .FirstOrDefault().Item1)
                                    .OrderBy(x => x.Item1)
                                    .Select(x =>
                                    new
                                    {
                                        Date = x.Item1,
                                        Quantity = x.Item3
                                    }
                                )
                        };

            var result = query.ToList();

            // Assert

            foreach (var item in result)
            {
                System.Console.WriteLine($"{item.Product}");
                foreach (var c in item.Lines)
                {
                    System.Console.WriteLine($"{c.Date} - {c.Quantity}");
                }
            }

            //foreach (var item in result)
            //{
            //    System.Console.WriteLine($"{item.Product} - {item?.Last.Date} - {item?.Last.Quantity}");
            //}

        }

        [Test]
        public void SelectMany_Test()
        {
            // Arrange
            var start = new List<(string, List<(string, string)>)> {
                {("A", new List<(string, string)> { ("1", "2"), ("3","4")}) },
                {("B", new List<(string, string)> { ("5", "6"), ("7", "8")}) },

            };

            // Act
            var query = from s in start
                        from e in s.Item2
                        select new
                        {
                            Id = s.Item1,
                            Value1 = e.Item1,
                            Value2 = e.Item2
                        };

            var result = query.ToList();

            // Assert

            foreach (var item in result)
            {
                System.Console.WriteLine($"{item.Id}- {item.Value1}- {item.Value2}");
            }
        }

        [Test]
        public void Linq_Max_Test()
        {
            // Arrange
            var list = new List<int>();
            list.Where(x => x > 1).DefaultIfEmpty(0).Max();
        }

        private static int x = 0;
        [Test]
        public void Task_Test()
        {
            // Arrange
            var task1 = Task.Run(async() => {
                await Task.Delay(2000);

                var task11 = Task.Run(() => throw new Exception("wrong !"));
                await task11;

                var v = Interlocked.Increment(ref x);
                Console.WriteLine($"From task 1 : {v}");
            } );

            var task2 = Task.Run(() => {
                var v = Interlocked.Increment(ref x);
                Console.WriteLine($"From task 2 : {v}");
            });

            try
            {
                Task.WaitAll(task1, task2);
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    Console.WriteLine(e.Message);
                }
            }

        }
    }
}
