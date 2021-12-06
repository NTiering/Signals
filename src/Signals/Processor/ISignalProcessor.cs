using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Processor
{
    public interface ISignalProcessor
    {
        Task<T> Process<T>(T signal) where T : ISignal;
        Task<T> Process<T>(T signal, CancellationToken token) where T : ISignal;
        IDictionary<Type, Type[]> WorkFlows { get; }

    }
}