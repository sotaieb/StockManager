using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Infrastructure.Services
{
    public class ProductService : IProductService
    {
        public ProductService()
        {
        }
        /// <summary>
        /// Retrieves products from store
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Product>> GetProductsFromStore()
        {
            var products = Store.Products.ToArray()
               .Select(x => x.Value.Value);

            return Task.FromResult(products);           
        }

        /// <summary>
        /// Adds product to store
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task AddProduct(Product product)
        {
            Guard.IsNotNull(product, nameof(product));
            Guard.IsNotNullOrEmpty(product.Ean, nameof(product));

            var succeeded = Store.Products.TryAdd(product.Ean, new Lazy<Product>(() => product));

            // No problem, concurrency is managed
            //if (!succeeded)
            //{
            //    throw new RequestException("Product exists !");
            //}
            return Task.CompletedTask;
        }

        /// <summary>
        /// Check if product exists
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public Task<bool> Exists(Product product)
        {
            var exists = Store.Products.TryGetValue(product.Ean, out var current);
            return Task.FromResult(exists);
        }
    }
}
