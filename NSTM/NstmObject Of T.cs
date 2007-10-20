/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

//TODO: FEATURE IDEA - implement blocking read

namespace NSTM
{
    internal interface INstmObject : INstmVersioned
    {
        object Value { get; set; }
        object CloneValue();
    }


    // shared object: can be involved in many tx and can be used by many threads
    internal class NstmObject<T> : INstmObject<T>, INstmObject
    {
        private long version = 0;
        private T value;


        static NstmObject()
        {
            // check if T implements ICloneable...
            foreach(Type interfaceType in typeof(T).GetInterfaces())
                if (interfaceType == typeof(ICloneable))
                    return;

            // ...if not, then we can still work with T if it´s a value type or string, because cloning them is easy
            if (!(typeof(T).IsValueType || typeof(T) == typeof(string)))
                throw new InvalidCastException(string.Format("Invalid type parameter! Cannot create NstmObject<T> for type {0}. It is neither a value type, nor a string, nor does it implement ICloneable.", typeof(T).Name));
        }


        internal NstmObject() {}

        internal NstmObject(T initialValue)
        {
            this.Write(initialValue);
        }


        #region INstmObject<T> Members

        public T Read()
        {
            return this.Read(NstmReadOption.ReadWrite);
        }

        public T Read(NstmReadOption option)
        {
            return this.Read(option, NstmMemory.Current);
        }

        internal T Read(NstmReadOption option, INstmTransaction tx)
        {
            if (tx != null)
                return (T)(tx as NstmTransaction).LogRead(this, option);
            else
                lock (this) 
                {
                    return this.value;
                }
        }


        public void Write(T value)
        {
            this.Write(value, NstmMemory.Current);
        }

        internal void Write(T value, INstmTransaction tx)
        {
            if (tx != null)
                (tx as NstmTransaction).LogWrite(this, value);
            else
            {
                lock (this)
                {
                    this.value = value;
                    this.version++;
                }

                Infrastructure.RetryTriggerList.Instance.NotifyRetriesForTrigger(this);
            }
        }

        #endregion


        #region INstmObject + INstmVersioned Members

        public long Version
        {
            get { return this.version; }
        }

        void INstmVersioned.IncrementVersion()
        {
            this.version++;
        }

        object INstmObject.Value
        {
            get { return this.value; }
            set { this.value = (T)value; }
        }

        object INstmObject.CloneValue()
        {
            if (typeof(T).IsValueType || typeof(T) == typeof(string))
                // value types and strings are cloned by just returning them
                // for value types that means they are implicitly copied,
                // and strings are immutable anyhow
                return this.value;
            else
                // if it´s not a value type or string, then it must be cloneable
                // (our class ctor has checked that!)
                if (this.value == null)
                    return default(T);
                else
                    return ((ICloneable)this.value).Clone();
        }
        #endregion
    }
}
