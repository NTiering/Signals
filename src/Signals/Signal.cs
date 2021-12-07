using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Signals
{
    public abstract class Signal : ISignal
    {
        public IEnumerable<Type> HandlersToSkip { get; set; } = Enumerable.Empty<Type>();

    }
}
