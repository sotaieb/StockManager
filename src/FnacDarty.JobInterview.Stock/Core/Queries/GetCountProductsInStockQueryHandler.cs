using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Queries
{
    /// <summary>
    /// Describes a request to retrieve products count in stock (qty > 0)
    /// </summary>
    public class GetCountProductsInStockQuery : IRequest
    {
        /// <summary>
        /// If date is null get the the count from the last state of stock
        /// </summary>
        public DateTime? Date { get; set; }
    }

    public class GetCountProductsInStockQueryHandler : IRequestHandler<GetCountProductsInStockQuery, int>
    {
        private readonly IStockService _stockService;

        public GetCountProductsInStockQueryHandler(IStockService stockService)            
        {
            this._stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }
        public async Task<int> Handle(GetCountProductsInStockQuery request, CancellationToken token)
        {
            var stock = await _stockService.GetStockByDate(request.Date);

            var productsInStock = stock.Count(x => x.Item2 > 0);
            return await Task.FromResult(productsInStock);
        }

        public Task<bool> Validate(GetCountProductsInStockQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request, nameof(request));
            return Task.FromResult(true);
        }
    }
}
