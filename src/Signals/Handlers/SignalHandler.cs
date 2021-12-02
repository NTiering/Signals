using Signals.Context;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Handlers
{
    public abstract class SignalHandler<T> : ISignalHandler where T : class, ISignal
    {
        public virtual int Order => 0;
        public virtual Type SignalType => typeof(T);
        public async Task Process(ISignal signal, ISignalContext context, CancellationToken token)
        {
            var s = signal as T;
            if (s == null) throw new InvalidOperationException($"Unable to cast {signal.GetType().FullName} as {typeof(T).FullName}");
            await OnSignal(s, context, token);
        }

        public async Task ProcessAbort(ISignal signal, ISignalContext context, CancellationToken token)
        {
            var s = signal as T;
            if (s == null) throw new InvalidOperationException($"Unable to cast {signal.GetType().FullName} as {typeof(T).FullName}");
            await OnSignalAbort(s, context, token);
        }

        protected virtual async Task OnSignal(T signal, ISignalContext context, CancellationToken token)
        {
            await Task.CompletedTask;
        }

        protected virtual async Task OnSignalAbort(T signal, ISignalContext context, CancellationToken token)
        {
            await Task.CompletedTask;
        }
    }






}
