/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Transactions;

namespace NSTM
{
    #region enum definitions
    public enum NstmTransactionScopeOption
    {
        Required,
        RequiresNew,
        RequiresNested, // default
        RequiredOrNested
    }

    public enum NstmTransactionIsolationLevel
    {
        ReadCommitted=0,  // data read in tx0 can be changed by another tx1 for tx0 to still commit successfully
        Serializable=1    // default; data read in tx0 must not be changed by another tx1 for tx0 to commit successfully; this pertains to data read and/or written in tx0
    }

    public enum NstmTransactionCloneMode
    {
        CloneOnWrite=0, // default - data of a tx0 object will be automatically cloned only if opened in ReadWrite mode; thus tx0 might see changes in data committed by other tx if it reads them several times
        CloneOnRead=1   // this ensures that tx0 does not even see any commtted changes by another tx on data it has read
    }

    public enum NstmSystemTransactionsMode
    {
        Ignore, // default
        EnlistOnBeginTransaction,
        EnlistOnAccess
    }
    #endregion


    public static class NstmMemory
    {
        private static NstmSystemTransactionsMode systemTransactionMode = NstmSystemTransactionsMode.Ignore;


        #region BeginTransaction()
        public static INstmTransaction BeginTransaction()
        {
            return NstmMemory.BeginTransaction(
                NstmTransactionScopeOption.RequiresNested, 
                NstmTransactionIsolationLevel.Serializable, 
                NstmTransactionCloneMode.CloneOnRead);
        }

        public static INstmTransaction BeginTransaction(NstmTransactionIsolationLevel isolationLevel)
        {
            return NstmMemory.BeginTransaction(
                NstmTransactionScopeOption.RequiresNested, 
                isolationLevel, 
                NstmTransactionCloneMode.CloneOnWrite);
        }


        public static INstmTransaction BeginTransaction(NstmTransactionScopeOption scopeOption, NstmTransactionIsolationLevel isolationLevel, NstmTransactionCloneMode cloneMode)
        {
            bool newTxCreated;
            return NstmMemory.BeginTransaction(scopeOption, isolationLevel, cloneMode, out newTxCreated);
        }


        public static INstmTransaction BeginTransaction(NstmTransactionScopeOption scopeOption, NstmTransactionIsolationLevel isolationLevel, NstmTransactionCloneMode cloneMode, out bool newTxCreated)
        {
            INstmTransaction tx = null;
            newTxCreated = true;

            Infrastructure.ThreadTransactionStack txStack = new NSTM.Infrastructure.ThreadTransactionStack();
            switch (scopeOption)
            {
                // check if there is an active tx on the thread. if so, don´t start a new tx, but return the current one
                case NstmTransactionScopeOption.Required:
                    if (txStack.Count == 0)
                        // no tx active, create a new one
                        tx = CreateTransaction(txStack, null, isolationLevel, cloneMode);
                    else
                    {
                        // there is an active tx...
                        tx = txStack.Peek();
                        if (tx.IsolationLevel < isolationLevel || tx.CloneMode < cloneMode)
                            // ...but the current tx does not match the requirements; create a new tx (not nested)
                            tx = CreateTransaction(txStack, null, isolationLevel, cloneMode);
                        else
                            newTxCreated = false;
                    }
                    break;
                
                // start a new independent tx in any case
                case NstmTransactionScopeOption.RequiresNew:
                    tx = CreateTransaction(txStack, null, isolationLevel, cloneMode);
                    break;
                
                // start a new tx which is nested in the current one
                case NstmTransactionScopeOption.RequiresNested:
                    tx = CreateTransaction(txStack, txStack.Peek(), isolationLevel, cloneMode);
                    break;

                // use existing tx if its compatible with current settings - otherwise create a nested tx
                case NstmTransactionScopeOption.RequiredOrNested:
                    if (txStack.Count == 0)
                        // no existing tx - create a new one
                        tx = CreateTransaction(txStack, null, isolationLevel, cloneMode);
                    else
                    {
                        // there is a tx: check if its settings are ok...
                        tx = txStack.Peek();
                        if (tx.IsolationLevel < isolationLevel || tx.CloneMode < cloneMode)
                            // ...the settings are not strict enough; create a new nested tx
                            tx = CreateTransaction(txStack, tx, isolationLevel, cloneMode);
                        else
                            newTxCreated = false;
                    }
                    break;
            }

            return tx;
        }

        internal static INstmTransaction CreateTransaction(Infrastructure.ThreadTransactionStack txStack, INstmTransaction txParent, NstmTransactionIsolationLevel isolationLevel, NstmTransactionCloneMode copyMode)
        {
            NstmTransaction tx = new NstmTransaction(txParent, isolationLevel, copyMode);

            if (NstmMemory.systemTransactionMode == NstmSystemTransactionsMode.EnlistOnBeginTransaction && Transaction.Current != null)
            {
                if (txStack.FindBySystemTransaction(Transaction.Current) == null)
                {
                    // no NSTM tx enlisted with current system.transaction yet
                    // 1. enlist new tx with system.transaction
                    tx.EnlistInSystemTransaction(Transaction.Current);
                    txStack.Push(tx);

                    // 2. create a nested tx within the new tx to be returned
                    // this tx can later be committed by the application code without making any changes visible yet.
                    // changes will then only be bubbled up to the new tx to await the final verdict from the system.transaction.
                    NstmTransaction txNested = new NstmTransaction(tx, NstmTransactionIsolationLevel.ReadCommitted, NstmTransactionCloneMode.CloneOnRead);
                    txStack.Push(txNested);

                    tx = txNested;
                }
                else
                    txStack.Push(tx);
            }
            else
                txStack.Push(tx);

            return tx;
        }
        #endregion


        #region ExecuteAtomically()
        public static void ExecuteAtomically(System.Threading.ThreadStart task)
        {
            ExecuteAtomically(false, task);
        }

        public static void ExecuteAtomically(bool autoRetry, System.Threading.ThreadStart task)
        {

            ExecuteAtomically(NstmTransactionScopeOption.RequiresNested,
                              NstmTransactionIsolationLevel.Serializable,
                              NstmTransactionCloneMode.CloneOnWrite,
                              autoRetry ? int.MaxValue : 0,
                              System.Threading.Timeout.Infinite,
                              System.Threading.Timeout.Infinite,
                              task);
        }

        public static void ExecuteAtomically(
            NstmTransactionScopeOption scope,
            NstmTransactionIsolationLevel isolationLevel,
            NstmTransactionCloneMode cloneMode,
            bool autoRetry,
            System.Threading.ThreadStart task)
        {
            ExecuteAtomically(scope,
                              isolationLevel,
                              cloneMode,
                              autoRetry ? int.MaxValue : 0,
                              System.Threading.Timeout.Infinite,
                              System.Threading.Timeout.Infinite,
                              task);
        }

        public static void ExecuteAtomically(
            NstmTransactionScopeOption scope,
            NstmTransactionIsolationLevel isolationLevel,
            NstmTransactionCloneMode cloneMode,
            int maxRetries,
            int sleepBeforeRetryMsec,
            int maxProcessingTimeMsec,
            System.Threading.ThreadStart task
            )
        {
            bool retryAtAll = maxRetries > 0 || maxProcessingTimeMsec != System.Threading.Timeout.Infinite;
            DateTime executionStart = DateTime.Now;
            int nRetries = 0;

            while (true)
            {
                NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction(scope, isolationLevel, cloneMode);
                try
                {
                    task.Invoke();

                    tx.Commit();
                    break;
                }
                catch (NstmRetryException ex)
                {
                    AutoResetEvent areRetry = new AutoResetEvent(false);
                    if (tx.ValidateForRetry(areRetry))
                    {
                        // only wait for changes to working set if validation succeeded
                        // because if it did not succeed then the manual retry was probably thrown on the basis of a wrong assumption
                        // this is to catch changes that would otherwise go unnoticed because of racing conditions
                        //TODO: THINK ABOUT - is this the best way to catch changes upon retry? (with calling ValidateForRetry() twice)
                        if (!areRetry.WaitOne(ex.Timeout, false))
                        {
                            if (tx.ValidateForRetry(null))
                            {
                                // only really abort retry if after a timeout nothing in the working set has changed
                                // this is to catch changes that would otherwise go unnoticed due to racing conditions
                                Infrastructure.RetryTriggerList.Instance.RemoveRetry(areRetry);
                                tx.Rollback();

                                // no changes to relevant tx values until timeout; abort retry!
                                throw new NstmRetryFailedException("Manual retry for transaction timed out! No changes were made to relevant transactional values.");
                            }
                        }
                    }

                    Infrastructure.RetryTriggerList.Instance.RemoveRetry(areRetry);
                    tx.Rollback();
                }
                catch (NstmValidationFailedException)
                {
                    tx.Rollback();

                    if (retryAtAll)
                    {
                        nRetries++;
                        if (nRetries > maxRetries)
                            throw new NstmRetryFailedException(string.Format("Could not commit transaction for operation within the limit of {0} retries!", maxRetries));

                        if (maxProcessingTimeMsec != System.Threading.Timeout.Infinite)
                        {
                            TimeSpan processingTime = DateTime.Now.Subtract(executionStart);
                            if (processingTime.Milliseconds > maxProcessingTimeMsec)
                                throw new NstmRetryFailedException(string.Format("Could not commit transaction for operation within the limit of {0} msec!", maxProcessingTimeMsec));
                        }
                    }
                    else
                        throw;
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }

                if (sleepBeforeRetryMsec != System.Threading.Timeout.Infinite)
                    System.Threading.Thread.Sleep(sleepBeforeRetryMsec);
            }
        }


        public static void Retry()
        {
            NstmMemory.Retry(1000); // default: timeout retry wait after 1sec
        }

        public static void Retry(int timeout)
        {
            throw new NstmRetryException(timeout);
        }

        #endregion


        #region Properties
        public static INstmTransaction Current
        {
            get
            {
                if (NstmMemory.systemTransactionMode == NstmSystemTransactionsMode.EnlistOnAccess && Transaction.Current != null)
                {
                    Infrastructure.ThreadTransactionStack txStack = new NSTM.Infrastructure.ThreadTransactionStack();
                    INstmTransaction tx = txStack.FindBySystemTransaction(Transaction.Current);
                    if (tx == null)
                    {
                        // there is no NSTM tx enlisted with the system.transaction, so a new NSTM tx needs to be created
                        tx = CreateTransaction(txStack, txStack.Peek(), NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnRead);
                        (tx as NstmTransaction).EnlistInSystemTransaction(Transaction.Current);
                        return tx;
                    }
                    else
                        // if there is already a NSTM tx enlisted with the system.transaction, then just return the current tx
                        return txStack.Peek();
                }
                else
                    return new Infrastructure.ThreadTransactionStack().Peek();
            }
        }


        public static int ActiveTransactionCount
        {
            get
            {
                return new Infrastructure.ThreadTransactionStack().Count;
            }
        }


        public static NstmSystemTransactionsMode SystemTransactionsMode
        {
            get { return NstmMemory.systemTransactionMode; }
            set { NstmMemory.systemTransactionMode = value; }
        }


        public static INstmObject<T> CreateObject<T>()
        {
            return new NstmObject<T>();
        }

        public static INstmObject<T> CreateObject<T>(T initialValue)
        {
            return new NstmObject<T>(initialValue);
        }
        #endregion
    }
}
