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
            this.entries.Add(txEntry.instance.GetHashCodeForVersion(), txEntry);
        }


        internal TransactionLogEntry this[INstmVersioned instance]
        {
            get
            {
                int key = instance.GetHashCodeForVersion();

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
                txLogClone.entries.Add(logEntry.instance.GetHashCodeForVersion(), (TransactionLogEntry)logEntry.Clone());
            return txLogClone;
        }

        #endregion
    }
}
