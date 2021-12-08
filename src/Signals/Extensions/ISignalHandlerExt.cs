using Signals.Context;
using Signals.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Extensions
{
    static class ISignalHandlerExt
    {
        public static async Task<IEnumerable<ISignalHandler>> RunSignalProcessors(this IEnumerable<ISignalHandler> handlers, ISignal signal, SignalContext context, CancellationToken token)
        {
            var ran = new List<ISignalHandler>();

            foreach (var handler in handlers)
            {
                if (signal.HandlersToSkip.OrEmpty().Contains(handler.GetType()))
                {
                    continue;
                }

                try
                {
                    ran.Add(handler);
                    await handler.Process(signal, context, token);
                    context.StepCount++;

                }
                catch (Exception ex)
                {
                    context.Exception = ex;
                    break;
                }
            }

            return ran.AsEnumerable();
        }

        public static async Task AbortSignalProcessors(this IEnumerable<ISignalHandler> handlers, ISignal signal, SignalContext context, CancellationToken token)
        {
            var h = handlers.ToArray().Reverse();

            foreach (var handler in h)
            {
                await handler.ProcessAbort(signal, context, token);
            }

        }
    }
}
