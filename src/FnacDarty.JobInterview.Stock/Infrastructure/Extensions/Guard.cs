using FnacDarty.JobInterview.Stock.Infrastructure.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FnacDarty.JobInterview.Stock.Infrastructure.Extensions
{
    public static class Guard
    {
        public static void IsNotNull(this object o, string name)
        {
            if (o == null)
            {
                throw new RequestException($"{name} cannot be blank");
            }
        }
       
        public static void IsNotNullOrEmpty(this string o, string message)
        {
            if (string.IsNullOrEmpty(o))
            {
                throw new RequestException(message);
            }
        }
        public static void IsNotEmptyCollection<T>(this IEnumerable<T> o, string message)
        {
            if (o == null || !o.Any())
            {
                throw new RequestException(message);
            }
        }
        public static void IsValidInterval(DateTime start, DateTime end)
        {
            if (start.Date > end.Date)
            {
                throw new RequestException("Invalid interval");
            }
        }
        public static void IsDateAnteriorThanToday(DateTime start)
        {
            if (start.Date > DateTime.Now.Date)
            {
                throw new RequestException("Invalid date");
            }
        }
    }
}
