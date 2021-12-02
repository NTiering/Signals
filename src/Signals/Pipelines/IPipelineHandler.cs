using System.Threading;
using System.Threading.Tasks;

namespace Signals.Pipelines
{
    public interface IPipelineHandler
    {
        Task ProcessStart(ISignal signal, CancellationToken token);
        Task ProcessEnd(ISignal signal, CancellationToken token);
    }


}
