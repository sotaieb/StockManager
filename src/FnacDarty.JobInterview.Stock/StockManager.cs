using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Commands;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Core.Queries;
using FnacDarty.JobInterview.Stock.Core.Services;
using FnacDarty.JobInterview.Stock.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock
{
    public class StockManager
    {
        #region Fields
        private readonly CancellationTokenSource _source = new CancellationTokenSource();
        private static readonly Lazy<StockManager> _stockManager = new Lazy<StockManager>(() => new StockManager());
        public static StockManager Instance { get { return _stockManager.Value; } }
       
        private readonly IRequestHandler<CreateFlowCommand, bool> _createFlowCommandHandler;
        private readonly IRequestHandler<CreateMultipleFlowCommand, bool> _createMultipleFlowCommandHandler;
        private readonly IRequestHandler<GetStockQuery, IEnumerable<(Product, int)>> _getStockQueryHandler;
        private readonly IRequestHandler<GetStockByProductQuery, (Product, int)> _getStockByProductQueryHandler;
        private readonly IRequestHandler<GetVariationQuery, IEnumerable<(Product, int)>> _getVariationQueryHandler;
        private readonly IRequestHandler<GetVariationByProductQuery, (Product, int)> _getVariationByProductQueryHandler;
        private readonly IRequestHandler<GetProductsInStockQuery, IEnumerable<(Product, int)>> _getProductsInStockQueryHandler;
        private readonly IRequestHandler<GetCountProductsInStockQuery, int> _getCountProductsInStockQueryHandler;
        #endregion

        #region Constructors
        private StockManager(IRequestHandler<CreateFlowCommand, bool> createFlowCommandHandler,
                            IRequestHandler<CreateMultipleFlowCommand, bool> createMultipleFlowCommandHandler,
                            IRequestHandler<GetStockQuery, IEnumerable<(Product, int)>> getStockQueryHandler,
                            IRequestHandler<GetStockByProductQuery, (Product, int)> getStockByProductQueryHandler,
                            IRequestHandler<GetVariationQuery, IEnumerable<(Product, int)>> getVariationQueryHandler,
                            IRequestHandler<GetVariationByProductQuery, (Product, int)> getVariationByProductQueryHandler,
                            IRequestHandler<GetProductsInStockQuery, IEnumerable<(Product, int)>> getProductsInStockQueryHandler,
                            IRequestHandler<GetCountProductsInStockQuery, int> getCountProductsInStockQueryHandler

            )
        {
            _createFlowCommandHandler = createFlowCommandHandler ?? throw new ArgumentNullException(nameof(createFlowCommandHandler));
            _createMultipleFlowCommandHandler = createMultipleFlowCommandHandler ?? throw new ArgumentNullException(nameof(createMultipleFlowCommandHandler));
            _getStockQueryHandler = getStockQueryHandler ?? throw new ArgumentNullException(nameof(getStockQueryHandler));
            _getStockByProductQueryHandler = getStockByProductQueryHandler ?? throw new ArgumentNullException(nameof(getStockByProductQueryHandler));
            _getVariationQueryHandler = getVariationQueryHandler ?? throw new ArgumentNullException(nameof(getVariationQueryHandler));
            _getVariationByProductQueryHandler = getVariationByProductQueryHandler ?? throw new ArgumentNullException(nameof(getVariationByProductQueryHandler));
            _getProductsInStockQueryHandler = getProductsInStockQueryHandler ?? throw new ArgumentNullException(nameof(getProductsInStockQueryHandler));
            _getCountProductsInStockQueryHandler = getCountProductsInStockQueryHandler ?? throw new ArgumentNullException(nameof(getCountProductsInStockQueryHandler));
        }

        private StockManager()
        {
            _createFlowCommandHandler = new CreateFlowCommandHandler(new ProductService(), new FlowService());
            _createMultipleFlowCommandHandler = new CreateMultipleFlowCommandHandler( new CreateFlowCommandHandler(new ProductService(), new FlowService()));
            _getStockQueryHandler = new GetStockQueryHandler(new StockService(new FlowService()));
            _getStockByProductQueryHandler = new GetStockByProductQueryHandler(new StockService(new FlowService()));
            _getVariationQueryHandler = new GetVariationQueryHandler(new StockService(new FlowService()));
            _getVariationByProductQueryHandler = new GetVariationByProductQueryHandler(new StockService(new FlowService()));
            _getProductsInStockQueryHandler = new GetProductsInStockQueryHandler(new StockService(new FlowService()));
            _getCountProductsInStockQueryHandler = new GetCountProductsInStockQueryHandler(new StockService(new FlowService()));
        }
        #endregion

        public async Task<bool> CreateFlow(DateTime date, string label, Product product, int quantity)
        {
            var request = new CreateFlowCommand
            {
                Date = date,
                Label = label,
                FlowLines = new List<ProductLine> {
                new ProductLine { Product = product, Quantity = quantity }},
            };
            
            return await _createFlowCommandHandler.Handle(request, _source.Token);
        }
        public async Task<bool> CreateFlow(DateTime date, string label, List<ProductLine> products)
        {
            var request = new CreateFlowCommand
            {
                Date = date,
                Label = label,
                FlowLines = products
            };

            return await _createFlowCommandHandler.Handle(request, _source.Token);
        }
        public async Task<bool> CreateMultipleFlow(DateTime date, string label, List<List<ProductLine>> MultiFlowLines)
        {
            // validate the hole flows
            // when an errror occurs, do not save anything
            var request = new CreateMultipleFlowCommand { 
                Date = date,
                Label = label,                
                MultiFlowLines = MultiFlowLines
            };


            return await _createMultipleFlowCommandHandler.Handle(request, _source.Token);
        }
        public async Task<bool> CreateInventoryFlow(string label, Product product, int quantity)
        {
            var request = new CreateFlowCommand
            {
                Date = DateTime.Today,
                Label = label,
                IsRegularizationFlow = true,
                FlowLines = new List<ProductLine> {
                new ProductLine { Product = product, Quantity = quantity }},

            };
          
            return await _createFlowCommandHandler.Handle(request, _source.Token);
        }
        public async Task<IEnumerable<(Product, int)>> GetStockByDate(DateTime? date=null)
        {
            var query = new GetStockQuery { Date = date };
            return await _getStockQueryHandler.Handle(query, _source.Token);
        }
        public async Task<(Product, int)> GetStockByProductAndDate(Product product, DateTime? date=null)
        {
            var query = new GetStockByProductQuery { Date = date, Product = product };
            return await _getStockByProductQueryHandler.Handle(query, _source.Token);
        }
        public async Task<IEnumerable<(Product, int)>> GetStockVariation(DateTime startDate, DateTime endDate)
        {
            var query = new GetVariationQuery { StartDate = startDate, EndDate = endDate };
            return await _getVariationQueryHandler.Handle(query, _source.Token);
        }
        public async Task<(Product, int)> GetStockVarationByProduct(Product product, DateTime startDate, DateTime endDate)
        {
            var query = new GetVariationByProductQuery { Product = product, StartDate = startDate, EndDate = endDate };
            return await _getVariationByProductQueryHandler.Handle(query, _source.Token);
        }
        public async Task<IEnumerable<(Product, int)>> GetProductsInStock(DateTime? date=null)
        {
            var query = new GetProductsInStockQuery { Date = date };
            return await _getProductsInStockQueryHandler.Handle(query, _source.Token);
        }
        public async Task<int> GetProductCount(DateTime? date=null)
        {
            var query = new GetCountProductsInStockQuery { Date = date };
            return await _getCountProductsInStockQueryHandler.Handle(query, _source.Token);
        }
    }
}
