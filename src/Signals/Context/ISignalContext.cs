using System;
using System.Collections.Generic;

namespace Signals.Context
{
    public interface ISignalContext
    {
        bool IsFirst { get; }
        bool IsLast { get; }
        int StepCount { get; }
        IEnumerable<Type> WorkFlow { get; }
        Exception Exception { get; }
    }
}