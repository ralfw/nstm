/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

/* Validation matrix
 *  validate on each read
 *    and on commit:            serializable+cloneOnWrite (ReadOnly values on read, ReadOnly+ReadWrite values on commit)
 *  validate on commit only:    seriablizable+cloneOnRead (ReadOnly+ReadWrite values)
 *  validate on commit,
 *    but only readOnly
 *    objects:                  readCommitted+cloneOnRead (ReadOnly values)
 *  no validation:              readCommitted+cloneOnWrite
 * 
 *  Validation means checking if a value has been changed by another tx. If that´s the case
 *  a tx needs to abort since it (possibly) worked with inconsistent data.
 */
 
using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using System.Transactions;
using System.Collections;

namespace NSTM
{
    // Not threadsafe since it´s supposed to be called only from one thread
    internal class NstmTransaction : INstmTransaction, IEnlistmentNotification
    {
        #region Data and Ctor
        private System.Transactions.Transaction txSystem;
        private NstmTransaction txParent;

        private NstmTransactionIsolationLevel isolationLevel;
        private NstmTransactionCloneMode cloneMode;

        private NstmTransactionActivityMode activityMode;

        private Infrastructure.TransactionLog txLog;


        internal NstmTransaction(INstmTransaction txParent, NstmTransactionIsolationLevel isolationLevel, NstmTransactionCloneMode cloneMode)
        {
            this.txParent = (NstmTransaction)txParent;
            if (txParent != null)
                // need to clone tx log so the inner tx can see all changes already made to the memory by outer tx
                this.txLog = (Infrastructure.TransactionLog)this.txParent.txLog.Clone();
            else
                this.txLog = new NSTM.Infrastructure.TransactionLog();

            this.isolationLevel = isolationLevel;
            this.cloneMode = cloneMode;

            this.activityMode = NstmTransactionActivityMode.Active;
        }
        #endregion


        #region log access to tx values in txlog
        internal object LogRead(INstmObject instance, NstmReadOption readOption)
        {
            lock (this)
            {
                lock (instance)
                {
                    Infrastructure.NstmObjectTransactionLogEntry txEntry = (Infrastructure.NstmObjectTransactionLogEntry)this.txLog[instance];
                    if (txEntry == null)
                    {
                        // first read on object: create txlog entry...

                        txEntry = new NSTM.Infrastructure.NstmObjectTransactionLogEntry(instance);
                        txLog.Add(txEntry);
                        txEntry.readOption = readOption;
                        // also register PassingReadOnly calls, since the read mode could be CloneOnRead!

                        // clone object if read happens in readwrite mode or cloneonread is switched on
                        if (txEntry.readOption == NstmReadOption.ReadWrite ||
                            this.cloneMode == NstmTransactionCloneMode.CloneOnRead)
                        {
                            txEntry.tempValue = instance.CloneValue();
                            // return cloned value
                            return txEntry.tempValue;
                        }
                        else
                            // return original value
                            return instance.Value;
                    }
                    else
                    {
                        // it has been read from the object before; there is already a txlog entry

                        // adjust readmode if it has changed since the last read
                        // but: the readOption can only be stepped up (from PassingRead to ReadOnly to ReadWrite)
                        if (txEntry.readOption < readOption) txEntry.readOption = readOption;

                        if (txEntry.tempValue != null)
                            return txEntry.tempValue;
                        else
                        {
                            if (this.ValidateObjectValueOnRead(instance, txEntry))
                            {
                                if (txEntry.readOption == NstmReadOption.ReadWrite)
                                {
                                    // if the readoption was stepped up from a lower level and there is no
                                    // clone yet, copy the current value now
                                    txEntry.tempValue = instance.CloneValue();
                                    return txEntry.tempValue;
                                }
                                else
                                    return instance.Value;
                            }
                            else
                                throw new NstmValidationFailedException("Cannot read NSTM transactional object value! Value has been changed by another transaction since it was last read. (Use isolation level 'ReadCommitted' to allow such changes to happen.)");
                        }
                    }
                }
            }
        }


        // Log read access to NstmTransactionalAttribute objects
        internal object LogRead(INstmVersioned instance, string fieldname, object currentValue)
        {
            lock (this)
            {
                lock (instance)
                {
                    Infrastructure.FieldlistTransactionLogEntry txEntry = (Infrastructure.FieldlistTransactionLogEntry)this.txLog[instance];
                    if (txEntry == null)
                    {
                        txEntry = new NSTM.Infrastructure.FieldlistTransactionLogEntry(instance);
                        txLog.Add(txEntry);
                        txEntry.readOption = NstmReadOption.ReadOnly;

                        if (this.cloneMode == NstmTransactionCloneMode.CloneOnRead)
                        {
                            // THINK ABOUT - is cloning necessary for field values?
                            // ANSWER: no. field values are either scalar, then adding them to tempFieldValues is a natural copy.
                            //         or they are an object reference. then their value is the reference which gets copied.
                            //         the object referenced can of course be transactional itself.
                            txEntry.tempFieldvalues.Add(fieldname, currentValue);
                            return txEntry.tempFieldvalues[fieldname];
                        }
                        else
                            return currentValue;
                    }
                    else
                    {
                        if (txEntry.tempFieldvalues.ContainsKey(fieldname))
                            return txEntry.tempFieldvalues[fieldname];
                        else
                        {
                            if (this.ValidateObjectValueOnRead(instance, txEntry))
                            {
                                if (txEntry.readOption == NstmReadOption.ReadWrite)
                                {
                                    // if the readoption was stepped up from a lower level and there is no
                                    // clone yet, copy the current value now
                                    // THINK ABOUT - is cloning necessary for field values? ANSER: see above.
                                    txEntry.tempFieldvalues.Add(fieldname, currentValue);
                                    return txEntry.tempFieldvalues[fieldname];
                                }
                                else
                                    return currentValue;
                            }
                            else
                                throw new NstmValidationFailedException("Cannot read NSTM transactional object value! Value has been changed by another transaction since it was last read. (Use isolation level 'ReadCommitted' to allow such changes to happen.)");
                        }
                    }
                }
            }
        }
        
        private bool ValidateObjectValueOnRead(INstmVersioned o, Infrastructure.TransactionLogEntry txEntry)
        {
            // validate on each read only if a.) serializability is asked for, and b.) the object´s data working set
            // could have changed since the last read. that´s only the case if clonemode=cloneOnWrite.
            // (no validation for PassingReadOnly reads!)
            if (txEntry.readOption > NstmReadOption.PassingReadOnly &&
                this.isolationLevel == NstmTransactionIsolationLevel.Serializable && this.cloneMode == NstmTransactionCloneMode.CloneOnWrite)
            {
                // isolevel "serializable": the value must not have changed since it was last read
                if (o.Version == txEntry.version)
                    return true;
                else
                    return false;
            }
            else
                // isolevel "readcommitted": the value is allowed to be changed by another tx
                return true;
        }


        // Log write for INstmObject<T> objects
        internal void LogWrite(INstmObject instance, object newValue)
        {
            lock (this)
            {
                Infrastructure.NstmObjectTransactionLogEntry txEntry = (Infrastructure.NstmObjectTransactionLogEntry)this.txLog[instance];
                if (txEntry == null)
                    lock (instance)
                    {
                        txEntry = new Infrastructure.NstmObjectTransactionLogEntry(instance);
                        txLog.Add(txEntry);
                    }

                txEntry.readOption = NstmReadOption.ReadWrite;
                txEntry.tempValue = newValue;
            }
        }


        // Log write access to NstmTransactionalAttribute objects from outside
        internal void LogWrite(INstmVersioned instance, string fieldname, object newValue)
        {
            lock (this)
            {
                Infrastructure.FieldlistTransactionLogEntry txEntry = (Infrastructure.FieldlistTransactionLogEntry)this.txLog[instance];
                if (txEntry == null)
                    lock (instance)
                    {
                        txEntry = new NSTM.Infrastructure.FieldlistTransactionLogEntry(instance);
                        txLog.Add(txEntry);
                    }
                txEntry.readOption = NstmReadOption.ReadWrite;
                txEntry.tempFieldvalues[fieldname] = newValue;
            }
        }

        // Log write access to NstmTransactionalAttribute objects during tx commit
        internal void LogWrite(INstmVersioned instance, Dictionary<string, object> newFieldValues)
        {
            lock (this)
            {
                Infrastructure.FieldlistTransactionLogEntry txEntry = (Infrastructure.FieldlistTransactionLogEntry)this.txLog[instance];
                if (txEntry == null)
                    lock (instance)
                    {
                        txEntry = new NSTM.Infrastructure.FieldlistTransactionLogEntry(instance);
                        txLog.Add(txEntry);
                    }
                txEntry.readOption = NstmReadOption.ReadWrite;
                txEntry.tempFieldvalues = newFieldValues;
            }
        }
        #endregion


        #region internal properties
        internal int LogCount
        {
            get
            {
                lock (this)
                {
                    return this.txLog.Count;
                }
            }
        }


        public System.Transactions.Transaction SystemTransaction
        {
            get
            {
                return this.txSystem;
            }
        }
        #endregion


        #region INstmTransaction Properties
        public NstmTransactionIsolationLevel IsolationLevel
        {
            get { return this.isolationLevel; }
        }

        public NstmTransactionCloneMode CloneMode
        {
            get { return this.cloneMode; }
        }

        public NstmTransactionActivityMode ActivityMode
        {
            get { return this.activityMode; }
        }

        public bool IsNested
        {
            get { return this.txParent != null; }
        }
        #endregion


        #region commit/rollback
        public void Commit()
        {
            this.Commit(true);
        }
        
        public bool Commit(bool throwExceptionOnFailure)
        {
            if (this.activityMode == NstmTransactionActivityMode.Active)
            {
                lock (this)
                {
                    if (this.PrepareLogEntries(throwExceptionOnFailure))
                        this.CommitLogEntries();
                    else
                        return false;
                }
            }

            return true;
        }


        private bool PrepareLogEntries(bool throwExceptionOnFailure)
        {
            Infrastructure.ThreadTransactionStack txStack = new NSTM.Infrastructure.ThreadTransactionStack();
            if (txStack.Peek() != this)
                if (throwExceptionOnFailure)
                    throw new InvalidOperationException("NSTM transaction to be committed is not the current transaction! Check for overlapping transaction Commit()/Abort(). Recommendation: Create transactions within the scope of a using() statement.");
                else
                    return false;

            // Validation / Preparation
            foreach (Infrastructure.TransactionLogEntry logEntry in this.txLog.SortedEntries)
            {
                switch (logEntry.readOption)
                {
                    case NstmReadOption.PassingReadOnly:
                        // do nothing; we don´t care if anything has been changed by another tx
                        break;

                    case NstmReadOption.ReadOnly:
                        if (!ValidateLogEntryOnCommit(logEntry, throwExceptionOnFailure, false))
                            return false; // abort commit, if validation failed. the tx then has been rolled back anyway.
                        break;

                    case NstmReadOption.ReadWrite:
                        if (!ValidateLogEntryOnCommit(logEntry, throwExceptionOnFailure, true))
                            return false;
                        logEntry.isLocked = true;
                        break;

                    //TODO: THINK ABOUT - should there be a fourth option "WriteOnly" with no validation? look at validation matrix!
                }
            }

            this.activityMode = NstmTransactionActivityMode.Committing;
            return true;
        }


        private void CommitLogEntries()
        {
            lock (this)
            {
                if (this.activityMode == NstmTransactionActivityMode.Committing)
                {
                    // list of retries to notify about changes committed by the tx
                    Infrastructure.RetryTriggerList.WaitingRetriesSet waitingRetriesForTx = new Infrastructure.RetryTriggerList.WaitingRetriesSet();

                    // Commit
                    //      for all acquired log entries commit the temp value to the object
                    //      no error must happen during this phase!
                    foreach (Infrastructure.TransactionLogEntry logEntry in this.txLog.SortedEntries)
                        if (logEntry.isLocked)
                        {
                            if (this.txParent == null)
                            {
                                // commit new value to object
                                logEntry.Commit();
                            }
                            else
                            {
                                // in nested tx don´t commit new value to object itself, 
                                // but just to enclosing tx´s log.
                                if (logEntry is Infrastructure.NstmObjectTransactionLogEntry)
                                    this.txParent.LogWrite((INstmObject)logEntry.instance, ((Infrastructure.NstmObjectTransactionLogEntry)logEntry).tempValue);
                                else
                                    this.txParent.LogWrite(logEntry.instance, ((Infrastructure.FieldlistTransactionLogEntry)logEntry).tempFieldvalues);
                            }

                            Monitor.Exit(logEntry.instance);
                            logEntry.isLocked = false;

                            // remember all retries to notify about this change
                            waitingRetriesForTx.Add(Infrastructure.RetryTriggerList.Instance.TakeAllRetries(logEntry.instance));
                        }

                    this.activityMode = NstmTransactionActivityMode.Committed;
                    new Infrastructure.ThreadTransactionStack().Remove(this);

                    // the tx is committed, now notify all retries waiting for changes made to transactional values
                    // by this tx
                    waitingRetriesForTx.NotifyAll();
                }
            }
        }


        private bool ValidateLogEntryOnCommit(Infrastructure.TransactionLogEntry logEntry, bool throwExceptionOnFailure, bool keepLogEntryLockedOnSuccess)
        {
            // check if value has not been changed by another tx since it was originally read
            // (see validation matrix at top of file)
            Monitor.Enter(logEntry.instance);
            if ((this.isolationLevel == NstmTransactionIsolationLevel.Serializable ||
                 (this.isolationLevel == NstmTransactionIsolationLevel.ReadCommitted &&
                  this.cloneMode == NstmTransactionCloneMode.CloneOnRead && 
                  logEntry.readOption == NstmReadOption.ReadOnly)
                 ) &&
                logEntry.version != logEntry.instance.Version)
            {
                // we´ve read another version than the current. some other tx must have
                // committed a change to the object. we need to abort this tx.
                Monitor.Exit(logEntry.instance);
                this.UnlockAcquiredLogEntries();
                this.Rollback();

                if (throwExceptionOnFailure)
                    throw new NstmValidationFailedException("Cannot commit NSTM transaction! Transactional object has been changed by another transaction. (Use the transaction isolation level 'ReadCommitted' to allow for such changes to be ok.)");
                else
                    return false;
            }
            else
            {
                if (!keepLogEntryLockedOnSuccess)
                    Monitor.Exit(logEntry.instance);

                return true;
            }
        }


        private void UnlockAcquiredLogEntries()
        {
            foreach (Infrastructure.TransactionLogEntry logEntry in this.txLog.SortedEntries)
                if (logEntry.isLocked)
                {
                    Monitor.Exit(logEntry.instance);
                    logEntry.isLocked = false;
                }
        }


        public void Rollback()
        {
            if (this.activityMode == NstmTransactionActivityMode.Active)
            {
                lock (this)
                {
                    this.txLog = new NSTM.Infrastructure.TransactionLog();

                    this.activityMode = NstmTransactionActivityMode.Aborted;
                    new Infrastructure.ThreadTransactionStack().Remove(this);
                }
            }
            else
            {
                // this is to help clearing the tx stack in case tx have been closed in the wrong order
                // and someone is now calling rollback in the correct order
                Infrastructure.ThreadTransactionStack txStack = new NSTM.Infrastructure.ThreadTransactionStack();
                if (txStack.Peek() == this)
                    txStack.Pop();
            }
        }


        internal bool ValidateForRetry(AutoResetEvent areRetry)
        {
            lock (this)
            {
                // register the retry waithandle with all values touched by the tx
                // if one of the values later is changed the retry waithandle is signaled
                bool txIsValid = true;
                foreach (Infrastructure.TransactionLogEntry logEntry in this.txLog.SortedEntries)
                {
                    txIsValid = txIsValid && logEntry.version == logEntry.instance.Version;
                    if (areRetry != null)
                        Infrastructure.RetryTriggerList.Instance.RegisterRetry(logEntry.instance, areRetry);
                }
                return txIsValid;
            }
        }


        //internal void RollbackForRetry(AutoResetEvent areRetry)
        //{
        //    if (this.activityMode == NstmTransactionActivityMode.Active)
        //    {
        //        lock (this)
        //        {
        //            // register the retry waithandle with all values touched by the tx
        //            // if one of the values later is changed the retry waithandle is signaled
        //            foreach (Infrastructure.TransactionLogEntry logEntry in this.txLog.SortedEntries)
        //                if (logEntry.readOption != NstmReadOption.PassingReadOnly)
        //                    Infrastructure.RetryTriggerList.Instance.RegisterRetry(logEntry.instance, areRetry);

        //            // now the real rollback can be done
        //            this.Rollback();
        //        }
        //    }
        //}
        #endregion


        #region System.Transactions handling
        internal void EnlistInSystemTransaction(System.Transactions.Transaction txSystem)
        {
            this.txSystem = txSystem;
            this.txSystem.EnlistVolatile(this, EnlistmentOptions.None);
        }

        #region IEnlistmentNotification Members
        public void Prepare(PreparingEnlistment preparingEnlistment)
        {
            if (this.PrepareLogEntries(false))
                preparingEnlistment.Prepared();
            else
                preparingEnlistment.ForceRollback();
        }


        public void Commit(Enlistment enlistment)
        {
            this.CommitLogEntries();
            enlistment.Done();
        }


        public void InDoubt(Enlistment enlistment)
        {
            enlistment.Done();
        }


        public void Rollback(Enlistment enlistment)
        {
            this.Rollback();
            enlistment.Done();
        }

        #endregion
        #endregion


        #region IDisposable Members

        public void Dispose()
        {
            this.Rollback();
        }

        #endregion
    }
}
