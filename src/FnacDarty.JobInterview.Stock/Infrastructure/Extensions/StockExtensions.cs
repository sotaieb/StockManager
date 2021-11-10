using FnacDarty.JobInterview.Stock.Core.Entities;
using System;

namespace FnacDarty.JobInterview.Stock.Infrastructure.Extensions
{
    public static class StockExtensions
    {
        public static bool IsValidProductLine(this ProductLine line, bool isRegularizationFLow)
        {
            if (line.Product == null
                || string.IsNullOrEmpty(line.Product.Ean)
                || line.Product.Ean.Length != 8
                || (isRegularizationFLow && line.Quantity < 0)
                )
            {

                return false;
            }
            return true;
        }

        public static string ToDateIdentifier(this DateTime date)
        {
            return date.ToString("yyyyMMdd");
        }
    }
}
