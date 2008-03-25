using System;
using System.Collections.Generic;
using System.Text;

using System.Transactions;

using NUnit.Framework;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testSystemTx
    {
        [Test]
        public void TestEnlistmentOnAccess()
        {
            INstmObject<int> o;

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.Ignore;
            using (TransactionScope tx = new TransactionScope())
            {
                o = NstmMemory.CreateObject<int>();
                o.Write(1);
            }

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnAccess;
            using (TransactionScope tx = new TransactionScope())
            {
                o = NstmMemory.CreateObject<int>();
                Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
                o.Write(1);
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
            }
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
        }


        [Test]
        public void TestEnlistmentOnBegin()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>();

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.Ignore;
            using (TransactionScope tx = new TransactionScope())
            {
                using (INstmTransaction txNstm = NstmMemory.BeginTransaction())
                {
                    o.Write(1);
                    txNstm.Commit();
                }
            }
            Assert.AreEqual(1, o.Read());

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnBeginTransaction;
            using (TransactionScope tx = new TransactionScope())
            {
                using (INstmTransaction txNstm = NstmMemory.BeginTransaction())
                {
                    o.Write(2);
                    txNstm.Commit();
                }
            }
            Assert.AreEqual(1, o.Read());
        }


        [Test]
        public void TestComplete()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>();
            Assert.AreEqual(0, o.Read());

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnAccess;
            using (TransactionScope tx = new TransactionScope())
            {
                o.Write(1);

                tx.Complete();
            }

            Assert.AreEqual(1, o.Read());
        }


        [Test]
        public void TestRollback()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>();

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnAccess;
            using (TransactionScope tx = new TransactionScope())
            {
                o = NstmMemory.CreateObject<int>();
                o.Write(1);
            }

            Assert.AreEqual(0, o.Read());
        }
    }
}
