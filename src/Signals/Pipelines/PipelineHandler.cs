using Signals.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Pipelines
{
    public abstract class PipelineHandler : IPipelineHandler
    {
        public virtual Task ProcessStart(ISignal signal, ISignalContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }
        public virtual Task ProcessEnd(ISignal signal, ISignalContext context, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }


}
