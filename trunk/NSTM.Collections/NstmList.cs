/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM.Collections
{
    public class NstmList<T>
    {
        private class Entry : ICloneable
        {
            public T data;
            public INstmObject<Entry> nextEntry, prevEntry;

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
                newEntry.prevEntry = this.prevEntry;

                return newEntry;
            }

            #endregion
        }


        INstmObject<Entry> noHeadRef, noTailRef;
        INstmObject<int> noCount;


        public NstmList()
        {
            this.noHeadRef = NstmMemory.CreateObject<Entry>(new Entry(default(T)));
            this.noTailRef = NstmMemory.CreateObject<Entry>(new Entry(default(T)));
            this.noCount = NstmMemory.CreateObject<int>(0);
        }


        public void Add(T item)
        {
            bool newTxCreated;
            INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite,
                                                    out newTxCreated);
            try
            {
                Entry eItem = new Entry(item);

                // get current end of list
                Entry eTailRef = this.noTailRef.Read(NstmReadOption.ReadWrite);
                INstmObject<Entry> noLastItem = eTailRef.nextEntry;

                // put new item after current end of list
                eItem.prevEntry = noLastItem;
                INstmObject<Entry> noNewLastItem = NstmMemory.CreateObject<Entry>(eItem);

                if (noLastItem != null)
                {
                    Entry eLastItem = noLastItem.Read(NstmReadOption.ReadWrite);
                    eLastItem.nextEntry = noNewLastItem;
                }

                // set new end of list
                eTailRef.nextEntry = noNewLastItem;

                // check if new item is the only one in the list. if so let the list head point to it.
                Entry eHeadRef = this.noHeadRef.Read(NstmReadOption.PassingReadOnly);
                if (eHeadRef.nextEntry == null)
                {
                    eHeadRef.nextEntry = noNewLastItem;
                    this.noHeadRef.Write(eHeadRef);
                }

                this.noCount.Write(noCount.Read(NstmReadOption.PassingReadOnly) + 1);

                if (newTxCreated) tx.Commit();
            }
            catch
            {
                if (newTxCreated) tx.Rollback();
                throw;
            }
        }


        public T this[int index]
        {
            get
            {
                T item;

                bool newTxCreated;
                INstmTransaction tx = NstmMemory.BeginTransaction(
                                                        NstmTransactionScopeOption.RequiresNested,
                                                        NstmTransactionIsolationLevel.ReadCommitted,
                                                        NstmTransactionCloneMode.CloneOnWrite,
                                                        out newTxCreated);
                try
                {
                    INstmObject<Entry> noCurrentItem = FindItemByIndex(index, true);
                    item = noCurrentItem.Read(NstmReadOption.ReadOnly).data;

                    if (newTxCreated) tx.Commit();
                }
                catch
                {
                    if (newTxCreated) tx.Rollback();
                    throw;
                }

                return item;
            }

            set
            {
                bool newTxCreated;
                INstmTransaction tx = NstmMemory.BeginTransaction(
                                                        NstmTransactionScopeOption.RequiresNested,
                                                        NstmTransactionIsolationLevel.ReadCommitted,
                                                        NstmTransactionCloneMode.CloneOnWrite,
                                                        out newTxCreated);
                try
                {
                    INstmObject<Entry> noCurrentItem = FindItemByIndex(index, true);
                    Entry eCurrentItem = noCurrentItem.Read(NstmReadOption.ReadWrite);
                    eCurrentItem.data = value;

                    if (newTxCreated) tx.Commit();
                }
                catch
                {
                    if (newTxCreated) tx.Rollback();
                    throw;
                }
            }
        }


        public void InsertBefore(int index, T item)
        {
            bool newTxCreated;
            INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite,
                                                    out newTxCreated);
            try
            {
                Entry eNewItem = new Entry(item);
                INstmObject<Entry> noNewItem = NstmMemory.CreateObject<Entry>(eNewItem);

                if (index <= 0)
                {
                    // prepend new item before current first item
                    Entry eHeadRef = this.noHeadRef.Read(NstmReadOption.ReadWrite);
                    eNewItem.nextEntry = eHeadRef.nextEntry;

                    if (eHeadRef.nextEntry != null)
                    {
                        // there was a current first item so let it become the second one by making the new one its predecessor
                        Entry eFirstItem = eHeadRef.nextEntry.Read(NstmReadOption.ReadWrite);
                        eFirstItem.prevEntry = noNewItem;
                    }
                    else
                    {
                        // there has been no item in the list so far. let the tail point to the new item, too.
                        Entry eTailRef = this.noTailRef.Read(NstmReadOption.ReadWrite);
                        eTailRef.nextEntry = noNewItem;
                    }

                    // let list head point to new first item
                    eHeadRef.nextEntry = noNewItem;

                    this.noCount.Write(noCount.Read(NstmReadOption.PassingReadOnly) + 1);
                }
                else if (index >= this.Count)
                {
                    // append new item
                    this.Add(item);
                }
                else
                {
                    // insert somewhere in the middle before current item
                    INstmObject<Entry> noCurrentItem = FindItemByIndex(index, true);
                    Entry eCurrentItem = noCurrentItem.Read(NstmReadOption.ReadWrite);

                    eNewItem.prevEntry = eCurrentItem.prevEntry;
                    eNewItem.nextEntry = noCurrentItem;

                    Entry ePrevItem = eCurrentItem.prevEntry.Read(NstmReadOption.ReadWrite);
                    ePrevItem.nextEntry = noNewItem;

                    eCurrentItem.prevEntry = noNewItem;

                    this.noCount.Write(noCount.Read(NstmReadOption.PassingReadOnly) + 1);
                }

                if (newTxCreated) tx.Commit();
            }
            catch
            {
                if (newTxCreated) tx.Rollback();
                throw;
            }
        }


        public bool RemoveAt(int index)
        {
            bool newTxCreated;
            INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite,
                                                    out newTxCreated);
            try
            {
                INstmObject<Entry> noCurrentItem = FindItemByIndex(index, false);

                if (noCurrentItem == null)
                {
                    if (newTxCreated) tx.Rollback();
                    return false;
                }

                // connect the item before the current one with the one after the current one
                Entry eCurrentItem = noCurrentItem.Read(NstmReadOption.PassingReadOnly);
                if (eCurrentItem.prevEntry != null)
                {
                    Entry ePrevItem = eCurrentItem.prevEntry.Read(NstmReadOption.ReadWrite);
                    ePrevItem.nextEntry = eCurrentItem.nextEntry;

                    if (ePrevItem.nextEntry == null)
                    {
                        // if there is no item after the current one then it´s the last one and the tail ref needs to be adjusted
                        Entry eTailRef = this.noTailRef.Read(NstmReadOption.ReadWrite);
                        eTailRef.nextEntry = eCurrentItem.prevEntry;
                    }
                }
                else
                {
                    // there is no item before the current one so it´s the first one; adjust the head ref
                    Entry eHeadRef = this.noHeadRef.Read(NstmReadOption.ReadWrite);
                    eHeadRef.nextEntry = eCurrentItem.nextEntry;
                }

                // connect the item after the current one with the one before it
                if (eCurrentItem.nextEntry != null)
                {
                    Entry eNextItem = eCurrentItem.nextEntry.Read(NstmReadOption.ReadWrite);
                    eNextItem.prevEntry = eCurrentItem.prevEntry;

                    if (eNextItem.prevEntry == null)
                    {
                        // there is no item before the current one so the head needs to be adjusted
                        Entry eHeadRef = this.noHeadRef.Read(NstmReadOption.ReadWrite);
                        eHeadRef.nextEntry = eCurrentItem.nextEntry;
                    }
                }
                else
                {
                    // there is no item after the current one so it´s the last one; adjust the tail ref
                    Entry eTailRef = this.noTailRef.Read(NstmReadOption.ReadWrite);
                    eTailRef.nextEntry = eCurrentItem.prevEntry;
                }

                this.noCount.Write(noCount.Read(NstmReadOption.PassingReadOnly) - 1);

                if (newTxCreated) tx.Commit();
            }
            catch
            {
                if (newTxCreated) tx.Rollback();
                throw;
            }

            return true;
        }


        private INstmObject<Entry> FindItemByIndex(int index, bool throwErrorOnIndexOutOfRange)
        {
            if (index >= 0 && index < this.noCount.Read(NstmReadOption.PassingReadOnly))
            {
                INstmObject<Entry> noCurrentItem = this.noHeadRef.Read(NstmReadOption.PassingReadOnly).nextEntry;
                int i = 0;
                while (i != index)
                {
                    Entry eCurrentItem = noCurrentItem.Read(NstmReadOption.PassingReadOnly);
                    noCurrentItem = eCurrentItem.nextEntry;
                    i++;
                }
                return noCurrentItem;
            }
            else
                if (throwErrorOnIndexOutOfRange)
                    throw new IndexOutOfRangeException("Index out of range!");
                else
                    return null;
        }


        public int Count
        {
            get
            {
                return this.noCount.Read(NstmReadOption.ReadOnly);
            }
        }
    }
}
