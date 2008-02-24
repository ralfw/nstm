/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM.Infrastructure
{
    internal class TransactionLog : ICloneable
    {
        private SortedDictionary<Guid, TransactionLogEntry> entries;


        internal TransactionLog()
        {
            this.entries = new SortedDictionary<Guid, TransactionLogEntry>();
        }


        internal void Add(TransactionLogEntry txEntry)
        {
            this.entries.Add(txEntry.instance.Id, txEntry);
        }


        internal TransactionLogEntry this[INstmVersioned instance]
        {
            get
            {
                if (this.entries.ContainsKey(instance.Id))
                    return this.entries[instance.Id];
                else
                    return null;
            }
        }


        internal int Count
        {
            get
            {
                return this.entries.Count;
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
            foreach (TransactionLogEntry logEntry in this.entries.Values)
                txLogClone.entries.Add(logEntry.instance.Id, (TransactionLogEntry)logEntry.Clone());
            return txLogClone;
        }
        #endregion
    }
}
