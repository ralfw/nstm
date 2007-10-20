/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM.Infrastructure
{
    internal class TransactionLog : ICloneable
    {
        private SortedDictionary<int, TransactionLogEntry> entries;


        internal TransactionLog()
        {
            entries = new SortedDictionary<int, TransactionLogEntry>();
        }


        internal void Add(TransactionLogEntry txEntry)
        {
            this.entries.Add(txEntry.instance.GetHashCode(), txEntry);
        }


        internal TransactionLogEntry this[object instance]
        {
            get
            {
                int key = instance.GetHashCode();

                if (entries.ContainsKey(key))
                    return entries[key];
                else
                    return null;
            }
        }


        internal int Count
        {
            get
            {
                return entries.Count;
            }
        }


        internal ICollection<TransactionLogEntry> SortedEntries
        {
            get
            {
                return this.entries.Values;
            }
        }


        #region ICloneable Members

        public object Clone()
        {
            TransactionLog txLogClone = new TransactionLog();
            foreach (TransactionLogEntry logEntry in entries.Values)
                txLogClone.entries.Add(logEntry.instance.GetHashCode(), (TransactionLogEntry)logEntry.Clone());
            return txLogClone;
        }

        #endregion
    }
}
