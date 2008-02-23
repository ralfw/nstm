using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM.Infrastructure
{
    internal abstract class TransactionLogEntry : ICloneable
    {
        private Guid id;

        internal INstmVersioned instance;
        internal long version = -1;

        internal bool isLocked = false;
        internal NstmReadOption readOption = NstmReadOption.PassingReadOnly;


        internal TransactionLogEntry(INstmVersioned instance)
        {
            this.id = Guid.NewGuid();

            this.instance = instance;
            this.version = instance.Version;
        }


        internal abstract void Commit();


        #region ICloneable Members

        public virtual object Clone()
        {
            throw new NotImplementedException("Method needs to be overridden in derived classes!");
        }

        #endregion


        public override int GetHashCode()
        {
            return this.id.GetHashCode();
        }
    }


    internal class NstmObjectTransactionLogEntry : TransactionLogEntry
    {
        internal object tempValue;


        internal NstmObjectTransactionLogEntry(INstmVersioned instance) : base(instance) { }


        internal override void Commit()
        {
            ((INstmObject)this.instance).Value = this.tempValue;
            this.instance.IncrementVersion();
        }


        #region ICloneable Members

        public override object Clone()
        {
            NstmObjectTransactionLogEntry logEntryClone = new NstmObjectTransactionLogEntry(this.instance);
            logEntryClone.isLocked = this.isLocked;
            logEntryClone.version = this.version;
            
            //TODO: THINK ABOUT - clone on txlog clone? some tests done run anymore if the value is cloned. check!
            //logEntryClone.tempValue = (this.tempValue as ICloneable).Clone();
            logEntryClone.tempValue = this.tempValue;

            logEntryClone.readOption = this.readOption;
            return logEntryClone;
        }

        #endregion
    }


    internal class FieldlistTransactionLogEntry : TransactionLogEntry
    {
        internal Dictionary<string, object> tempFieldvalues;


        internal FieldlistTransactionLogEntry(INstmVersioned instance)
            : base(instance)
        {
            this.tempFieldvalues = new Dictionary<string, object>();
        }


        internal override void Commit()
        {
            Type t = this.instance.GetType();
            foreach (string key in this.tempFieldvalues.Keys)
            {
                System.Reflection.FieldInfo fi = t.GetField(key, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                fi.SetValue(this.instance, this.tempFieldvalues[key]);
            }
            this.instance.IncrementVersion();
        }


        #region ICloneable Members

        public override object Clone()
        {
            FieldlistTransactionLogEntry logEntryClone = new FieldlistTransactionLogEntry(this.instance);
            logEntryClone.isLocked = this.isLocked;
            logEntryClone.version = this.version;

            logEntryClone.tempFieldvalues = new Dictionary<string, object>(this.tempFieldvalues);
                // the dict needs to be cloned when the txlog is cloned; otherwise changes in the nested tx are registered in the same dicts as in the parent tx.

            logEntryClone.readOption = this.readOption;
            return logEntryClone;
        }

        #endregion
    }
}
