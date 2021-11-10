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
    /// Describes a request to retrieve the variation of stock by product between two dates
    /// </summary>
    public class GetVariationByProductQuery : IRequest
    {
        public Product Product { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class GetVariationByProductQueryHandler: IRequestHandler<GetVariationByProductQuery, (Product, int)>
    {
        private readonly IStockService _stockService;

        public GetVariationByProductQueryHandler(IStockService stockService)
        {
            this._stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }
        public async Task<(Product, int)> Handle(GetVariationByProductQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request, nameof(request));

            await Validate(request, token);

            var start = await _stockService.GetStockByProductAndDate(request.Product, request.StartDate);
            var end = await _stockService.GetStockByProductAndDate(request.Product, request.EndDate);

            return await Task.FromResult((request.Product, end.Item2 - start.Item2));
        }

        public Task<bool> Validate(GetVariationByProductQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request.Product, nameof(request));
            Guard.IsNotNullOrEmpty(request.Product.Ean, nameof(request));
            Guard.IsValidInterval(request.StartDate, request.EndDate);
            return Task.FromResult(true);
        }
    }
}
