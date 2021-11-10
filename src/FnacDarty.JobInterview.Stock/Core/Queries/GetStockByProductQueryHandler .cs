using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Queries
{
    /// <summary>
    /// Describes a request to retrieve the stock state by product and date
    /// </summary>
    public class GetStockByProductQuery : IRequest
    {
        public Product Product { get; set; }
        public DateTime? Date { get; set; }
    }

    public class GetStockByProductQueryHandler : IRequestHandler<GetStockByProductQuery, (Product, int)>
    {
        private readonly IStockService _stockService;

        public GetStockByProductQueryHandler(IStockService stockService)
        {
            this._stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }
        public async Task<(Product, int)> Handle(GetStockByProductQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request, nameof(request));

            if (!request.Date.HasValue)
            {
                request.Date = DateTime.Today;
            }

            return await _stockService.GetStockByProductAndDate(request.Product, request.Date.Value);
        }

        public Task<bool> Validate(GetStockByProductQuery request, CancellationToken token)
        {
            return Task.FromResult(true);
        }
    }
}
