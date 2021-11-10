namespace FnacDarty.JobInterview.Stock.Core.Entities
{
    /// <summary>
    /// Describes a product line
    /// </summary>
    public class ProductLine
    {
        /// <summary>
        /// The <see cref="Product"/> informations
        /// </summary>
        public Product Product { get; set; }

        /// <summary>
        /// Quantity of product
        /// </summary>
        public int Quantity { get; set; }
    }
}