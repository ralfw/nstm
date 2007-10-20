using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Laos;
using PostSharp.Extensibility;

namespace NSTM
{
    [NstmTransactional]
    public class NstmTransactional<T>
    {
        private T value;


        static NstmTransactional()
        {
            if (!(typeof(T).IsValueType || typeof(T) == typeof(string)))
                throw new InvalidCastException(string.Format("Invalid type parameter! Cannot create NstmTransactional<T> for type {0}. It is neither a value type, nor a string.", typeof(T).Name));
        }


        public NstmTransactional(T value)
        {

            this.value = value;
        }


        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                this.value = value;
            }
        }

        
        public static implicit operator T(NstmTransactional<T> instance)
        {
            return instance.value;
        }

        public static implicit operator NstmTransactional<T>(T value)
        {
            return new NstmTransactional<T>(value);
        }
    }
}
