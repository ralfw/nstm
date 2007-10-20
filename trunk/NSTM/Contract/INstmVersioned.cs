using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM
{
    public interface INstmVersioned
    {
        long Version { get; }
        void IncrementVersion();
    }
}
