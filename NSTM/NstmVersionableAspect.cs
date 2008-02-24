using System;
using System.Collections.Generic;
using System.Text;

using PostSharp.Laos;

namespace NSTM
{
    internal class NstmVersion : INstmVersioned
    {
        private Guid id = Guid.NewGuid();
        private long version = 0;

        #region IVersioned Members
        Guid INstmVersioned.Id
        {
            get { return this.id; }
        }

        long INstmVersioned.Version
        {
            get
            {
                return this.version;
            }
        }

        void INstmVersioned.IncrementVersion()
        {
            this.version++;
        }
        #endregion
    }


    [Serializable]
    internal class NstmVersionableAspect : CompositionAspect
    {
        public override object CreateImplementationObject(InstanceBoundLaosEventArgs eventArgs)
        {
            return new NstmVersion();
        }

        public override Type GetPublicInterface(Type containerType)
        {
            return typeof(INstmVersioned);
        }
    }
}
