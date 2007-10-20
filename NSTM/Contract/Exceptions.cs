using System;
using System.Collections.Generic;
using System.Text;

namespace NSTM
{
    [Serializable]
    public class NstmValidationFailedException : ApplicationException
    {
        public NstmValidationFailedException() : base() { }
        
        public NstmValidationFailedException(string message) : base(message) { }

        public NstmValidationFailedException(string message, Exception innerException) : base(message, innerException) { }

        public NstmValidationFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class NstmRetryFailedException : ApplicationException
    {
        public NstmRetryFailedException() : base() { }

        public NstmRetryFailedException(string message) : base(message) { }

        public NstmRetryFailedException(string message, Exception innerException) : base(message, innerException) { }

        public NstmRetryFailedException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }


    [Serializable]
    public class NstmRetryException : ApplicationException
    {
        private int timeout = System.Threading.Timeout.Infinite;

        public NstmRetryException() : base() { }

        public NstmRetryException(int timeout)
            : base()
        {
            this.timeout = timeout;
        }

        public int Timeout
        {
            get { return this.timeout; }
        }
    }

}
