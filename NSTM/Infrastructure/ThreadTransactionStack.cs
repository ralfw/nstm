/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

using System.Transactions;
using System.Threading;

namespace NSTM.Infrastructure
{
    /*
     * Tx are fixed to the thread they are created in.
     * All tx still open in a thread are pointed to by a TLS slot ("NstmTxStack").
     * In this slot they are mainly organized like a stack, because the default is to really nest tx if they
     * are created while another tx is still open.
     * However tx need not to nest; there can be just multiple tx open side by side in a thread.
     * Finishing side by side tx needs to be in reverse order, though, and must not overlap.
     */
    internal class ThreadTransactionStack
    {
        private const string TLS_TX_STACK_SLOTNAME = "NstmTxStack";

        private List<INstmTransaction> txStack;


        internal ThreadTransactionStack()
        {
            LocalDataStoreSlot txStackTLSSlot = Thread.GetNamedDataSlot(ThreadTransactionStack.TLS_TX_STACK_SLOTNAME);
            this.txStack = (List<INstmTransaction>)Thread.GetData(txStackTLSSlot);
            if (this.txStack == null)
            {
                this.txStack = new List<INstmTransaction>();
                Thread.SetData(txStackTLSSlot, this.txStack);
            }
        }


        public void Push(INstmTransaction tx)
        {
            this.txStack.Add(tx);
        }
        

        public INstmTransaction Pop()
        {
            if (this.txStack.Count > 0)
            {
                INstmTransaction tx = this.txStack[this.txStack.Count - 1];
                this.txStack.RemoveAt(this.txStack.Count - 1);
                return tx;
            }
            else
                return null;
        }


        public INstmTransaction Peek()
        {
            if (this.txStack.Count > 0)
                return this.txStack[this.txStack.Count - 1];
            else
                return null;
        }


        public INstmTransaction FindBySystemTransaction(System.Transactions.Transaction txSystem)
        {
            for (int i = this.txStack.Count - 1; i >= 0; i--) // start at end of list to go up the stack and check most recent tx first
            {
                INstmTransaction tx = this.txStack[i];
                if (tx.SystemTransaction == txSystem)
                    return tx;
            }

            return null;
        }


        public void Remove(INstmTransaction tx)
        {
            if (this.Peek() == tx)
                this.Pop();
            else
                throw new InvalidOperationException("NSTM transaction to be removed is not the current transaction! Check for overlapping transaction Commit()/Abort(). Recommendation: Create transactions within the scope of a using() statement.");
        }


        public int Count
        {
            get
            {
                return this.txStack.Count;
            }
        }
    }
}
