using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NSTM;

namespace NSTM.BlackboxTests
{  
    [TestFixture]
    public class testNstmMemoryWithTxStack
    {
        [Test]
        public void TestSingleTx()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
        }


        [Test]
        public void TestRequiresNestedTx()
        {
            // create nested tx
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                using (INstmTransaction tx1 = NstmMemory.BeginTransaction())
                {
                    Assert.AreEqual(2, NstmMemory.ActiveTransactionCount);
                    Assert.IsTrue(tx1.IsNested);
                }
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
        }


        [Test]
        public void TestRequiredTx()
        {
            bool newTxCreated;

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite, out newTxCreated))
                {
                    Assert.IsFalse(newTxCreated);
                    Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                    Assert.IsFalse(tx1.IsNested);
                    Assert.AreEqual(tx0, tx1);
                }
                Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);


            //-- tx required: no tx active & current tx has compatible settings for second tx
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnRead))
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
                {
                    Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                }
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            //-- tx required: no tx active & current tx does not match requirements
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnRead))
                {
                    Assert.AreEqual(2, NstmMemory.ActiveTransactionCount);
                }
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
        }


        [Test]
        public void TestRequiresNewTx()
        {
            bool newTxCreated;

            // create new, independent tx while another is still active
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite, out newTxCreated))
                {
                    Assert.IsTrue(newTxCreated);
                    Assert.AreEqual(2, NstmMemory.ActiveTransactionCount);
                    Assert.IsFalse(tx1.IsNested);
                    Assert.AreNotEqual(tx0, tx1);
                }
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
        }


        [Test]
        public void TestRequiredOrNested()
        {
            bool newTxCreated = false;

            // inner tx compatible with outer; no new tx created
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite, out newTxCreated))
            {
                Assert.IsTrue(newTxCreated);

                using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiredOrNested, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite, out newTxCreated))
                {
                    Assert.IsFalse(newTxCreated);
                    Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                    Assert.IsFalse(tx1.IsNested);
                    Assert.AreEqual(tx0, tx1);
                }
                Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
            }

            // inner tx not compatible with outer; create new nested tx
            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.Required, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiredOrNested, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnRead, out newTxCreated))
                {
                    Assert.IsTrue(newTxCreated);
                    Assert.AreEqual(2, NstmMemory.ActiveTransactionCount);
                    Assert.IsTrue(tx1.IsNested);
                    Assert.AreNotEqual(tx0, tx1);
                }
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
            }
        }


        [Test]
        public void TestCloseTxInWrongOrder()
        {
            INstmTransaction txB;
            INstmTransaction txA = NstmMemory.BeginTransaction();
            txB = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite);
            try
            {
                txA.Commit(); // this is wrong; the tx created first needs to be finished last (FILO)
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("overlapping") >= 0);
                txB.Rollback();
                txA.Rollback();
            }
        }
    }
}
