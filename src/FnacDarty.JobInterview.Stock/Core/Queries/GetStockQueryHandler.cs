using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Queries
{
    /// <summary>
    /// Describes a request to retrieve the stock state by date
    /// </summary>
    public class GetStockQuery : IRequest
    {
        public DateTime? Date { get; set; }
    }

    public class GetStockQueryHandler : IRequestHandler<GetStockQuery, IEnumerable<(Product, int)>>
    {
        private readonly IStockService _stockService;

        public GetStockQueryHandler(IStockService stockService)
        {
            this._stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
        }
        public async Task<IEnumerable<(Product, int)>> Handle(GetStockQuery request, CancellationToken token)
        {
            await Validate(request, token);
            
            return await _stockService.GetStockByDate(request.Date);
        }

        public Task<bool> Validate(GetStockQuery request, CancellationToken token)
        {
            Guard.IsNotNull(request, nameof(request));
            
            return Task.FromResult(true);
        }
    }
}
