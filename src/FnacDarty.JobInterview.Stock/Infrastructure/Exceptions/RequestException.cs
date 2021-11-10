using System;
using System.Collections.Generic;
using System.Text;

namespace FnacDarty.JobInterview.Stock.Infrastructure.Exceptions
{
    public class RequestException: Exception
    {
        public RequestException(string message): base(message)
        {
        }
    }
}
