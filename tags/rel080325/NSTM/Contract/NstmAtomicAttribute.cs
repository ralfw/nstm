/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

/* PostSharp.Laos:
 *      Copyright © by Gael Fraiteur and the postsharp.org Community. Some rights reserved. 
 */

using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Laos;

namespace NSTM
{
    //TODO: FEATURE-IDEA - use ExecuteAtomically in NstmAtomicAttribute
    [Serializable]
    [AttributeUsage(AttributeTargets.Method | 
                    AttributeTargets.Property | 
                    AttributeTargets.Constructor)]
    public class NstmAtomicAttribute : OnMethodBoundaryAspect
    {
        private NstmTransactionScopeOption transactionScope;
        private NstmTransactionIsolationLevel isolationLevel;
        private NstmTransactionCloneMode cloneMode;


        public NstmAtomicAttribute() 
        { 
            this.transactionScope = NstmTransactionScopeOption.Required;
            this.isolationLevel = NstmTransactionIsolationLevel.Serializable;
            this.cloneMode = NstmTransactionCloneMode.CloneOnRead;
        }

        public NstmAtomicAttribute(NstmTransactionIsolationLevel isolationLevel)
        {
            this.isolationLevel = isolationLevel;
        }

        public NstmAtomicAttribute(NstmTransactionScopeOption transactionScope,
                                   NstmTransactionIsolationLevel isolationLevel,
                                   NstmTransactionCloneMode cloneMode)
        {
            this.transactionScope = transactionScope;
            this.isolationLevel = isolationLevel;
            this.cloneMode = cloneMode;
        }


        public override void OnEntry(MethodExecutionEventArgs eventArgs)
        {
            bool createdNewTx;

            NstmMemory.BeginTransaction(this.transactionScope, 
                                        this.isolationLevel, 
                                        this.cloneMode, 
                                        out createdNewTx);

            eventArgs.InstanceTag = createdNewTx;
        }


        public override void OnExit(MethodExecutionEventArgs eventArgs)
        {
            bool createdNewTx = (bool)eventArgs.InstanceTag;

            if (createdNewTx)
                if (eventArgs.Exception == null)
                    NstmMemory.Current.Commit();
                else
                    NstmMemory.Current.Rollback();
        }
    }
}
