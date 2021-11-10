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
    /// Describes a command to create multiple flow
    /// </summary>
    public class CreateMultipleFlowCommand : AbstractCreateFlowCommand
    {
        public List<List<ProductLine>> MultiFlowLines { get; set; }
    }

    
    
    /// <summary>
    /// Create multiple flow handler
    /// </summary>
    public class CreateMultipleFlowCommandHandler : IRequestHandler<CreateMultipleFlowCommand, bool>
    {
        private readonly IRequestHandler<CreateFlowCommand, bool> _createSingleFlow;

        public CreateMultipleFlowCommandHandler(IRequestHandler<CreateFlowCommand, bool> createSingleFlow)            
        {
            this._createSingleFlow = createSingleFlow ?? throw new ArgumentNullException(nameof(createSingleFlow));
        }      

        public async Task<bool> Handle(CreateMultipleFlowCommand request, CancellationToken token)
        {
            var multi = new List<CreateFlowCommand>();
            foreach (var flowLines in request.MultiFlowLines)
            {
                var command = new CreateFlowCommand
                { 
                    Date = request.Date,
                    Label = request.Label,
                    IsRegularizationFlow = request.IsRegularizationFlow,
                    FlowLines = flowLines
                };
                await _createSingleFlow.Validate(command, token);
                multi.Add(command);
            }

            foreach (var command in multi)
            {              
                await _createSingleFlow.Handle(command, token);
            }

            return await Task.FromResult(true);
        }

        public async Task<bool> Validate(CreateMultipleFlowCommand request, CancellationToken token = default)
        {
            return await Task.FromResult(true);
        }
        
    }
}