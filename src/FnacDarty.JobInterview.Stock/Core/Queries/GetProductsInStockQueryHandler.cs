using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Queries
{
    /// <summary>
    /// Describes a request to retrieve products in stock (qty > 0)
    /// </summary>
    public class GetProductsInStockQuery : IRequest
    {
        public DateTime? Date { get; set; }
    }

    public class GetProductsInStockQueryHandler : IRequestHandler<GetProductsInStockQuery, IEnumerable<(Product, int)>>
    {
        private readonly IStockService _stockService;

        public GetProductsInStockQueryHandler(IStockService stockService)
        {
            this._stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }
        public async Task<IEnumerable<(Product, int)>> Handle(GetProductsInStockQuery request, CancellationToken token)
        {
            await Validate(request, token);

            var stock = await _stockService.GetStockByDate(request.Date);

            var productsInStock = stock.Where(x => x.Item2 > 0);
            return await Task.FromResult(productsInStock);

        }

        public Task<bool> Validate(GetProductsInStockQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request, nameof(request));

            return Task.FromResult(true);
        }
    }
}
