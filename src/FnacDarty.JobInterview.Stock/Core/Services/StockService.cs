using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Services
{
    /// <summary>
    /// Stock queries
    /// </summary>
    public class StockService : IStockService
    {
        private readonly IFlowService _flowService;
        private readonly object lockObject = new object();

        public StockService(IFlowService flowService)
        {
            _flowService = flowService ?? throw new ArgumentNullException(nameof(flowService));
        }

        /// <summary>
        /// Gets stock by date using flows history
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<IEnumerable<(Product, int)>> GetStockByDate(DateTime? date = null)
        {

            if (date.HasValue)
            {
                Guard.IsDateAnteriorThanToday(date.Value);
            }
            else {
                date = DateTime.Now;
            }

            var flowsInStore = await _flowService.GetFlowsFromStore();

            var productLines = from s in flowsInStore
                               from l in s.FlowLines
                               where s.Date.Date <= date.Value.Date
                               select new
                               {
                                   s.Date,
                                   s.IsRegularizationFlow,
                                   l.Product,
                                   l.Product.Ean,
                                   l.Quantity
                               };

            var stock = from s in productLines
                        group s by s.Ean into g
                        select (g.FirstOrDefault().Product, 
                                g.Where(x => x.Date >= g.Where(y => y.IsRegularizationFlow)
                                 .Select(z=> z.Date)
                                 .OrderByDescending(z => z.Date)
                                 .FirstOrDefault())
                                 .Select(x => x.Quantity)
                                 .Aggregate((sum, v) => sum + v));

            return await Task.FromResult(stock);
        }

        /// <summary>
        /// Gets stock state bye date and product using flow history.
        /// </summary>
        /// <param name="product"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public async Task<(Product, int)> GetStockByProductAndDate(Product product, DateTime? date = null)
        {
            Guard.IsNotNull(product, nameof(product));
            Guard.IsNotNullOrEmpty(product.Ean, "Product EAN is required");
            if (date.HasValue)
            {
                Guard.IsDateAnteriorThanToday(date.Value);
            }
            else
            {
                date = DateTime.Now;
            }

            var flowsInStore = await _flowService.GetFlowsFromStore();

            var productLines = from s in flowsInStore
                               from l in s.FlowLines
                               where s.Date.Date <= date.Value.Date && l.Product.Ean == product.Ean
                               select new
                               {
                                   s.Date,
                                   s.IsRegularizationFlow,
                                   l.Quantity
                               };

            var stock = (from s in productLines
                         where s.Date >= productLines.Where(y => y.IsRegularizationFlow)
                                                      .Select(z => z.Date)
                                                      .OrderByDescending(z => z.Date)
                                                      .FirstOrDefault().Date

                         select s.Quantity)
                        .Aggregate((sum, v) => sum + v);

            return await Task.FromResult((product, stock));
        }
    }
}
