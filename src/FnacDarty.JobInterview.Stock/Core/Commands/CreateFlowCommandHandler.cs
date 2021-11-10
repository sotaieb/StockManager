using FnacDarty.JobInterview.Stock.Core.Abstrations;
using FnacDarty.JobInterview.Stock.Core.Entities;
using FnacDarty.JobInterview.Stock.Infrastructure.Exceptions;
using FnacDarty.JobInterview.Stock.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FnacDarty.JobInterview.Stock.Core.Commands
{
    /// <summary>
    /// Describes a command to create a single flow
    /// </summary>

    public class CreateFlowCommand : AbstractCreateFlowCommand
    {
        public List<ProductLine> FlowLines { get; set; }
    }
    public abstract class AbstractCreateFlowCommand : IRequest
    {
        public DateTime Date { get; set; }
        public string Label { get; set; }
        public bool IsRegularizationFlow { get; set; }

    }

    /// <summary>
    /// Create flow handler
    /// </summary>
    public class CreateFlowCommandHandler : IRequestHandler<CreateFlowCommand, bool>
    {
        private readonly IProductService _productService;
        private readonly IFlowService _flowService;

        public CreateFlowCommandHandler(IProductService productService,
                                        IFlowService flowService)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _flowService = flowService ?? throw new ArgumentNullException(nameof(flowService));
        }

        public async Task<bool> Handle(CreateFlowCommand request, CancellationToken token)
        {
            await Validate(request, token);

            if (request.IsRegularizationFlow)
                request.Date = DateTime.Today;


            foreach (var item in request.FlowLines)
            {
                if (!await _productService.Exists(item.Product))
                {
                    await _productService.AddProduct(item.Product);
                }
            }

            var flow = new Flow
            {
                Date = request.Date,
                Label = request.Label,
                IsRegularizationFlow = request.IsRegularizationFlow,
                FlowLines = request.FlowLines
            };
            await _flowService.AddFlow(flow);

            return await Task.FromResult(true);
        }

        public async Task<bool> Validate(CreateFlowCommand request, CancellationToken token = default)
        {
            Guard.IsNotNull(request, nameof(request));

            Guard.IsNotNullOrEmpty(request.Label, "Flow Label is required");
            Guard.IsNotEmptyCollection(request.FlowLines, "Invalid product list");

            if (request.IsRegularizationFlow && request.FlowLines.Count > 1)
            {
                throw new RequestException("Regularization flow should have unique product.");
            }
            if (request.IsRegularizationFlow && request.FlowLines[0].Quantity < 0)
            {
                throw new RequestException("Regularization shouhld have a valid quantity");
            }
            if (request.FlowLines.Any(x => !x.IsValidProductLine(request.IsRegularizationFlow)))
            {
                throw new RequestException("Invalid product line, check your product ean (8 characters) and quantity.");
            }
            if (request.FlowLines.GroupBy(x => x.Product.Ean).Any(x => x.Count() > 1))
            {
                throw new RequestException("Duplicate product detected.");
            }
            foreach (var item in request.FlowLines)
            {
                if (item.Quantity == 0)
                {
                    throw new RequestException("Invalid product line quantity.");
                }
                var lastInventoryDate = await _flowService.GetLastInventoryDate(item.Product);
                if (request.Date < lastInventoryDate)
                {
                    throw new RequestException("Invalid flow date, it should be after inventory date.");
                }
            }

            return await Task.FromResult(true);
        }
    }
}