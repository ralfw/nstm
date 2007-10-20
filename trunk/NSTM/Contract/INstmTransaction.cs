/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;
using System.Transactions;

namespace NSTM
{
    public enum NstmTransactionActivityMode
    {
        Active,
        Committing,
        Committed,
        Aborted
        //Preempted
    }


    public interface INstmTransaction : IDisposable
    {
        NstmTransactionIsolationLevel IsolationLevel { get; }
        NstmTransactionCloneMode CloneMode { get; }
        NstmTransactionActivityMode ActivityMode { get; }
        bool IsNested { get; }
        Transaction SystemTransaction { get; }

        void Commit();
        bool Commit(bool throwExceptionOnFailure);
        void Rollback();
    }
}
