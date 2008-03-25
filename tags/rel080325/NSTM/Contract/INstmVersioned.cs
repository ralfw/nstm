using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM
{
    public interface INstmVersioned
    {
        Guid Id { get; }

        long Version { get; }
        void IncrementVersion();
    }
}
