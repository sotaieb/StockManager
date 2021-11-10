using FnacDarty.JobInterview.Stock.Core.Entities;
using System;
using System.Collections.Concurrent;

namespace FnacDarty.JobInterview.Stock
{
    public class Store
    {
        /// <summary>
        /// Contains product list
        /// <string, Product>: Product identifier, <see cref="Product"/> informations
        /// </summary>
        public static ConcurrentDictionary<string, Lazy<Product>> Products { get; set; } =
           new ConcurrentDictionary<string, Lazy<Product>>();

        /// <summary>
        /// Contains the flow list
        /// </summary>
        public static ConcurrentDictionary<Guid, Lazy<Flow>> Flows { get; set; } =
            new ConcurrentDictionary<Guid, Lazy<Flow>>();
    }
}
