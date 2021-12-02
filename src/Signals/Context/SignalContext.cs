using Signals.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Signals.Context
{
    internal class SignalContext : ISignalContext
    {
        public SignalContext(IEnumerable<Type> WorkFlow)
        {
            this.WorkFlow = WorkFlow.OrEmpty();
        }

        public bool IsFirst => StepCount == 0;
        public bool IsLast => WorkFlow.Any() && WorkFlow.Count() - 1 == StepCount;
        public IEnumerable<Type> WorkFlow { get; }
        public int StepCount { get; internal set; }

    }
}
