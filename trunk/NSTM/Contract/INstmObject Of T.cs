/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM
{
    public enum NstmReadOption
    {
        // need to be indexed in increasing order to be able to escalate an object´s read option
        PassingReadOnly = 0, 
        ReadOnly = 1,
        ReadWrite = 2 // default; the other options are optimizations; with ReadWrite a user of NSTM is on the safe side - however it is the slowest option since it requires a clone of the object´s data
    }


    public interface INstmObject<T>
    {
        long Version { get; }

        T Read();
        T Read(NstmReadOption option);
        void Write(T value);
    }
}
