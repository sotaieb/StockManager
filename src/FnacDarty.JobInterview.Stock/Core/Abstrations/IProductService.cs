using FnacDarty.JobInterview.Stock.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Abstrations
{
    /// <summary>
    /// Product service contract
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Retrieves products from store
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Product>> GetProductsFromStore();

        /// <summary>
        /// Creates a product
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task AddProduct(Product product);

        /// <summary>
        /// Check if product exists
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<bool> Exists(Product product);
    }
}