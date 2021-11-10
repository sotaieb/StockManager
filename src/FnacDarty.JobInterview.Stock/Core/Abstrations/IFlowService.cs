using FnacDarty.JobInterview.Stock.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Abstrations
{
    /// <summary>
    /// Flow service contract(stock movement)
    /// </summary>
    public interface IFlowService
    {
        /// <summary>
        /// Retrieves flows from the store
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<Flow>> GetFlowsFromStore();

        /// <summary>
        /// Creates a flow (movement)
        /// </summary>
        /// <param name="flow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task AddFlow(Flow flow, CancellationToken token = default);

        /// <summary>
        /// Creates a list of flow
        /// </summary>
        /// <param name="flows"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task AddFlow(List<Flow> flows, CancellationToken token = default);

        /// <summary>
        /// Retrieves last inventory date
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<DateTime> GetLastInventoryDate(Product product, CancellationToken token = default);
    }
}