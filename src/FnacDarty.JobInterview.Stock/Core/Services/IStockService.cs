using FnacDarty.JobInterview.Stock.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Services
{
    public interface IStockService
    {
        Task<IEnumerable<(Product, int)>> GetStockByDate(DateTime? date = null);
        Task<(Product, int)> GetStockByProductAndDate(Product product, DateTime? date = null);
    }
}