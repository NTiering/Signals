using Signals.Context;
using Signals.Extensions;
using Signals.Handlers;
using Signals.Pipelines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Processor
{
    public class SignalProcessor : ISignalProcessor
    {
        private readonly IDictionary<Type, ISignalHandler[]> signalHandlers;
        private readonly IEnumerable<IPipelineHandler> pipelineHandlers = Enumerable.Empty<IPipelineHandler>();

        public SignalProcessor(IEnumerable<ISignalHandler> signalHandlers)
        {
            this.signalHandlers = GetSignalHandlers(signalHandlers.OrEmpty());
        }

        public SignalProcessor(IEnumerable<ISignalHandler> signalHandlers, IEnumerable<IPipelineHandler> pipelineHandlers) 
            : this(signalHandlers)
        {
            this.pipelineHandlers = pipelineHandlers.OrEmpty();
        }

        public IDictionary<Type, Type[]> WorkFlows => signalHandlers.ToDictionary(x => x.Key, x => x.Value.Select(y => y.GetType()).ToArray());

        public async Task<T> Process<T>(T signal) where T : ISignal
        {
            await Process(signal, new CancellationToken());
            return signal;
        }

        public async Task<T> Process<T>(T signal, CancellationToken token) where T : ISignal
        {
            if (signal == null) throw new ArgumentNullException(nameof(signal));
            if (NoHandlersPresent<T>()) throw new InvalidOperationException($"No handlers found for Type of {typeof(T).FullName}");

            await RunStartPipelineHandlers(signal, token);
            await RunSignalProcessors(signal, token);
            await RunEndPipelineHandlers(signal, token);

            return signal;
        }

        private async Task RunStartPipelineHandlers<T>(T signal, CancellationToken token) where T : ISignal
        {

            if (pipelineHandlers.Empty()) return;
            foreach (var handler in pipelineHandlers)
            {
                await handler.ProcessStart(signal, token);
            }
        }

        private async Task RunEndPipelineHandlers<T>(T signal, CancellationToken token) where T : ISignal
        {
            if (pipelineHandlers.Empty()) return;
            foreach (var handler in pipelineHandlers)
            {
                await handler.ProcessEnd(signal, token);
            }
        }

        private async Task RunSignalProcessors<T>(T signal, CancellationToken token) where T : ISignal
        {
            var handlers = signalHandlers[typeof(T)];
            var ran = new List<ISignalHandler>();
            Exception exception = null;
            var context = new SignalContext(handlers.Select(x => x.GetType()));

            foreach (var handler in handlers)
            {
                if (signal.HandlersToSkip != null && signal.HandlersToSkip.Contains(handler.GetType()))
                {
                    continue;
                }

                ran.Add(handler);
                try
                {
                    await handler.Process(signal, context, token);
                    context.StepCount++;

                }
                catch (Exception ex)
                {
                    exception = ex;
                    break;
                }
            }

            if (exception == null) return;
            ran.Reverse();
            foreach (var handler in ran)
            {
                await handler.ProcessAbort(signal, context, token);
            }

            throw exception;
        }

        private bool NoHandlersPresent<T>()
        {
            var rtn = !signalHandlers.Keys.Contains(typeof(T));
            return rtn;
        }

        private static IDictionary<Type, ISignalHandler[]> GetSignalHandlers(IEnumerable<ISignalHandler> handlers)
        {
            var rtn = new Dictionary<Type, ISignalHandler[]>();

            var types = handlers.Select(x => x.SignalType).Distinct();
            foreach (var type in types)
            {
                rtn[type] = handlers
                    .OrderBy(x => x.Order)
                    .Where(x => x.SignalType == type)
                    .ToArray();
            }

            return rtn;
        }
    }
}
