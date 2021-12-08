using Signals.Context;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Pipelines
{
    public interface IPipelineHandler
    {
        Task ProcessStart(ISignal signal, ISignalContext context, CancellationToken token);
        Task ProcessEnd(ISignal signal, ISignalContext context, CancellationToken token);
    }


}
