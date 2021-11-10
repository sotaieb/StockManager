using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Abstrations
{
    public interface IRequestHandler<in T, U> where T : IRequest
    {
        Task<U> Handle(T request, CancellationToken token);
        Task<bool> Validate(T request, CancellationToken token);
    }

    public abstract class RequestHandler<T, U> : IRequestHandler<T, U> where T : IRequest
    {
        protected abstract Task<U> Handle(T request);
        protected abstract Task<bool> Validate(T request);
        public Task<U> Handle(T request, CancellationToken token) => Handle(request);
        public Task<bool> Validate(T request, CancellationToken token) => Validate(request);       
    }
}
