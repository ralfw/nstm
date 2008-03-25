/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Collections;

namespace NSTM.Infrastructure
{
#if DEBUG
    public // this is necessary for the test of this internal type, since NUnit requires test fixtures to be public
#else
    internal 
#endif
        partial class RetryTriggerList
    {
        #region set of waiting retries for multiple triggers
        public class WaitingRetriesSet
        {
            private Hashtable waitingRetries = new Hashtable();

            public void Add(Hashtable waitingRetriesForInstance)
            {
                foreach (AutoResetEvent areRetry in waitingRetriesForInstance.Values)
                    if (!this.waitingRetries.ContainsKey(areRetry))
                        this.waitingRetries.Add(areRetry, areRetry);
            }

            public void NotifyAll()
            {
                foreach (AutoResetEvent areRetry in this.waitingRetries.Values)
                    areRetry.Set();
            }
        }
        #endregion


        #region managing the singleton
        private static Infrastructure.RetryTriggerList singletonInstance = new NSTM.Infrastructure.RetryTriggerList();

        public static RetryTriggerList Instance
        {
            get
            {
                return RetryTriggerList.singletonInstance;
            }
        }
        #endregion


        #region manage the waiting retries for a list of tx value triggers
        // manages the list of tx values (instances) and their retries (waithandles) waiting for changes applied to them
        private Hashtable triggers;

        public RetryTriggerList()
        {
            Clear();
        }

        private void Clear()
        {
            lock (this)
            {
                this.triggers = new Hashtable();
            }
        }


        public void RegisterRetry(object instance, AutoResetEvent areRetry)
        {
            lock (this)
            {
                Hashtable waitingRetries;

                if (this.triggers.ContainsKey(instance))
                {
                    // there are already retries waiting for this instance
                    // add this retry´s waithandle to its list
                    waitingRetries = (Hashtable)this.triggers[instance];
                    if (!waitingRetries.ContainsKey(areRetry))
                        waitingRetries.Add(areRetry, areRetry);
                }
                else
                {
                    // no retries waiting for instance so far
                    // create and entry for it in the trigger list
                    waitingRetries = new Hashtable();
                    waitingRetries.Add(areRetry, areRetry);
                    this.triggers.Add(instance, waitingRetries);
                }
            }
        }


        public Hashtable TakeAllRetries(object instance)
        {
            lock (this)
            {
                if (this.triggers.ContainsKey(instance))
                {
                    Hashtable waitingRetries = (Hashtable)this.triggers[instance];
                    this.triggers.Remove(instance);
                    return waitingRetries;
                }
                else
                    return new Hashtable();
            }
        }


        public void RemoveRetry(AutoResetEvent areRetry)
        {
            lock (this)
            {
                foreach (Hashtable waitingRetries in this.triggers.Values)
                    waitingRetries.Remove(areRetry);
            }
        }


        public void NotifyRetriesForTrigger(object instance)
        {
            WaitingRetriesSet wrs = new WaitingRetriesSet();
            wrs.Add(this.TakeAllRetries(instance));
            wrs.NotifyAll();
        }
        #endregion
    }
}
