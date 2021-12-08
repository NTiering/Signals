using Signals.Context;
using Signals.Pipelines;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Extensions
{
    static class IPipelineHandlerExt
    {
        public static async Task RunEndPipelineHandlers<T>(this IEnumerable<IPipelineHandler> pipelineHandlers, T signal, ISignalContext context, CancellationToken token) where T : ISignal
        {
            if (pipelineHandlers.IsEmpty()) return;
            foreach (var handler in pipelineHandlers)
            {
                await handler.ProcessEnd(signal, context, token);
            }
        }

        public static async Task RunStartPipelineHandlers<T>(this IEnumerable<IPipelineHandler> pipelineHandlers, T signal, ISignalContext context, CancellationToken token) where T : ISignal
        {
            if (pipelineHandlers.IsEmpty()) return;
            foreach (var handler in pipelineHandlers)
            {
                await handler.ProcessStart(signal, context, token);
            }
        }
    }
}
