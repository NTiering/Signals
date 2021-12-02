using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Signals.Processor
{
    public interface ISignalProcessor
    {
        Task Process<T>(T signal) where T : ISignal;
        Task Process<T>(T signal, CancellationToken token) where T : ISignal;
        IDictionary<Type, Type[]> WorkFlows { get; }

    }
}