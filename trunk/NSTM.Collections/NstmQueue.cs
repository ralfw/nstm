/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM.Collections
{
    public class NstmQueue<T>
    {
        private class Entry : ICloneable
        {
            public T data;
            public INstmObject<Entry> nextEntry;

            public Entry(T data)
            {
                this.data = data;
            }

            #region ICloneable Members

            public object Clone()
            {
                Entry newEntry = new Entry(default(T));
                newEntry.data = this.data;
                newEntry.nextEntry = this.nextEntry;

                return newEntry;
            }

            #endregion
        }


        INstmObject<Entry> headRef, tailRef;
        INstmObject<int> count;


        public NstmQueue()
        {
            this.headRef = NstmMemory.CreateObject<Entry>(new Entry(default(T)));
            this.tailRef = NstmMemory.CreateObject<Entry>(new Entry(default(T)));
            this.count = NstmMemory.CreateObject<int>(0);
        }


        public void Enqueue(T item)
        {
            bool newTxCreated;
            INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite,
                                                    out newTxCreated);
            try
            {
                INstmObject<Entry> newTailEntry = NstmMemory.CreateObject<Entry>(new Entry(item));

                // append new entry to tail of queue
                Entry tailRefEntry = this.tailRef.Read(NstmReadOption.ReadWrite);
                if (tailRefEntry.nextEntry != null)
                {
                    Entry currentTailEntry = tailRefEntry.nextEntry.Read(NstmReadOption.ReadWrite);
                    currentTailEntry.nextEntry = newTailEntry;
                }
                tailRefEntry.nextEntry = newTailEntry;

                // check if the queue head needs to be set since the queue has been empty so far
                Entry headRefEntry = this.headRef.Read(NstmReadOption.PassingReadOnly);
                if (headRefEntry.nextEntry == null)
                {
                    headRefEntry.nextEntry = newTailEntry;
                    this.headRef.Write(headRefEntry); // write explicitly, since it was opened in 'PassingReadOnly' mode for performance reasons
                }

                this.count.Write(count.Read(NstmReadOption.PassingReadOnly) + 1);

                if (newTxCreated) tx.Commit();
            }
            catch
            {
                if (newTxCreated) tx.Rollback();
                throw;
            }
        }


        public T Dequeue()
        {
            bool newTxCreated;
            INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite,
                                                    out newTxCreated);
            try
            {
                Entry headEntry;

                // get entry from head of queue
                Entry headRefEntry = this.headRef.Read(NstmReadOption.ReadWrite);
                if (headRefEntry.nextEntry == null)
                    throw new InvalidOperationException("NSTM queue empty!");

                headEntry = headRefEntry.nextEntry.Read(NstmReadOption.PassingReadOnly);

                // advance head to next entry
                headRefEntry.nextEntry = headEntry.nextEntry; 
                
                // check if queue is now empty...
                if (headRefEntry.nextEntry == null)
                {
                    // ...no next entry in queue, so the queue is empty and the tail needs to be adjusted
                    Entry tailRefEntry = this.tailRef.Read(NstmReadOption.ReadWrite);
                    tailRefEntry.nextEntry = null;
                }

                this.count.Write(count.Read(NstmReadOption.PassingReadOnly) - 1);

                if (newTxCreated) tx.Commit();

                return headEntry.data;
            }
            catch
            {
                if (newTxCreated) tx.Rollback();
                throw;
            }
        }


        public T Peek()
        {
            bool newTxCreated;
            INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite,
                                                    out newTxCreated);
            try
            {
                Entry headEntry;

                // get entry from head of queue
                Entry headRefEntry = this.headRef.Read(NstmReadOption.PassingReadOnly);
                if (headRefEntry.nextEntry == null)
                    throw new InvalidOperationException("NSTM queue empty!");

                headEntry = headRefEntry.nextEntry.Read(NstmReadOption.PassingReadOnly);

                if (newTxCreated) tx.Commit();

                return headEntry.data;
            }
            catch
            {
                if (newTxCreated) tx.Rollback();
                throw;
            }
        }


        public int Count
        {
            get
            {
                return this.count.Read(NstmReadOption.ReadOnly);
            }
        }
    }
}
