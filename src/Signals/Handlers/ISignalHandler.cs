using Signals.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Handlers
{
    public interface ISignalHandler
    {
        int Order { get; }
        Type SignalType { get; }
        Task Process(ISignal signal, ISignalContext context, CancellationToken token);
        Task ProcessAbort(ISignal signal, ISignalContext context, CancellationToken token);
    }
}
