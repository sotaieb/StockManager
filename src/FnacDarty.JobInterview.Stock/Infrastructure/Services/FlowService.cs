using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Infrastructure.Exceptions;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Infrastructure.Services
{
    /// <summary>
    /// Flow service (stock movement)
    /// </summary>
    public class FlowService : IFlowService
    {
        /// <summary>
        /// Retrieves flows from the store
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<Flow>> GetFlowsFromStore()
        {
            var flows = Store.Flows.ToArray()
                .Select(x => x.Value.Value);
            
            return Task.FromResult(flows);
        }

        /// <summary>
        /// Creates a flow (movement)
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task AddFlow(Flow flow, CancellationToken token = default)
        {
            Guard.IsNotNull(flow, nameof(flow));

            var succeeded = Store.Flows.TryAdd(flow.Id, new Lazy<Flow>(() => flow)); 
                
            if (!succeeded)
            {
                throw new RequestException("Flow exists !");
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates a list of flow
        /// </summary>
        /// <param name="flows"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public Task AddFlow(List<Flow> flows, CancellationToken token = default)
        {
            // should be a transaction here
            foreach (var item in flows)
            {
                AddFlow(item);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves last inventory date
        /// </summary>
        /// <param name="product"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<DateTime> GetLastInventoryDate(Product product, CancellationToken token = default)
        {
            Guard.IsNotNull(product, nameof(product));
            Guard.IsNotNullOrEmpty(product.Ean, nameof(product));

            var flows = await GetFlowsFromStore();

            if (flows == null)
            {
                return await Task.FromResult(default(DateTime));
            }

            var date = flows.Where(x => x.IsRegularizationFlow &&
                                        x.FlowLines[0].Product.Ean == product.Ean)
                            .Select(x => x.Date)
                            .DefaultIfEmpty(default)
                            .Max();

            return await Task.FromResult(date);
        }
    }
}
