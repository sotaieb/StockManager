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
    /// Describes a request to retrieve the variation of stock between two dates
    /// </summary>
    public class GetVariationQuery : IRequest
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }

    public class GetVariationQueryHandler : IRequestHandler<GetVariationQuery, IEnumerable<(Product, int)>>
    {
        private readonly IStockService _stockService;

        public GetVariationQueryHandler(IStockService stockService)
        {
            this._stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }
        public async Task<IEnumerable<(Product, int)>> Handle(GetVariationQuery request, CancellationToken token)
        {
            await Validate(request, token);

            var start = await _stockService.GetStockByDate(request.StartDate);
            var end = await _stockService.GetStockByDate(request.EndDate);

            var products = start.Select(x => x.Item1).Union(end.Select(x => x.Item1));
            var query = from id in products
                        join e in end on id equals e.Item1 into g1
                        from j1 in g1.DefaultIfEmpty((id, 0))
                        join s in start on j1.Item1 equals s.Item1 into g2
                        from j2 in g2.DefaultIfEmpty((id, 0))
                        select (id, j1.Item2 - j2.Item2);

            return await Task.FromResult(query);
        }

        public Task<bool> Validate(GetVariationQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request, nameof(request));

            Guard.IsValidInterval(request.StartDate, request.EndDate);
            return Task.FromResult(true);
        }
    }
}
