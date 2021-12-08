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
            return await Process(signal, new CancellationToken());
        }

        public async Task<T> Process<T>(T signal , CancellationToken token) where T : ISignal
        {            
            if (signal == null) throw new ArgumentNullException(nameof(signal));
            if (NoHandlersPresent<T>()) throw new InvalidOperationException($"No handlers found for Type of {typeof(T).FullName}");
       
            var handlers = signalHandlers[typeof(T)];
            var context = new SignalContext(handlers.Select(x => x.GetType()));

            await pipelineHandlers.RunStartPipelineHandlers(signal, context, token);
            var handlersRan = await handlers.RunSignalProcessors(signal, context, token);
            if (context.HasException)
            {
                await handlersRan.AbortSignalProcessors(signal, context, token);
            }
            await pipelineHandlers.RunEndPipelineHandlers(signal, context, token);

            if (context.HasException) throw context.Exception;

            return signal;            
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
