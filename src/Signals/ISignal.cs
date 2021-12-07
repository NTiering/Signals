using System;
using System.Collections.Generic;

namespace Signals
{
    public interface ISignal
    {
        IEnumerable<Type> HandlersToSkip { get;  }
    }
}
