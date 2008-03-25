/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace NSTM.Collections.BlackboxTests
{
    [TestFixture]
    public class testNstmQueue
    {
        [Test]
        public void TestEnqueueAndCount()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmQueue<int> q = new NstmQueue<int>();
            Assert.AreEqual(0, q.Count);

            q.Enqueue(1);
            Assert.AreEqual(1, q.Count);
            q.Enqueue(2);
            Assert.AreEqual(2, q.Count);
            q.Enqueue(3);
            Assert.AreEqual(3, q.Count);
        }


        [Test]
        public void TestDequeueAndCount()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmQueue<int> q = new NstmQueue<int>();
            q.Enqueue(1);
            q.Enqueue(2);
            q.Enqueue(3);

            Assert.AreEqual(1, q.Dequeue());
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual(2, q.Dequeue());
            Assert.AreEqual(1, q.Count);
            Assert.AreEqual(3, q.Dequeue());
            Assert.AreEqual(0, q.Count);

            try
            {
                q.Dequeue();
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("empty") >= 0);
            }
        }


        [Test]
        public void TestPeek()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmQueue<int> q = new NstmQueue<int>();
            q.Enqueue(1);

            Assert.AreEqual(1, q.Peek());

            q.Dequeue();

            try
            {
                q.Peek();
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("empty") >= 0);
            }
        }


        [Test]
        public void TestSystemTransactions()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmQueue<int> q = new NstmQueue<int>();
            q.Enqueue(1);

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.Ignore;
            using (System.Transactions.TransactionScope tx = new System.Transactions.TransactionScope())
            {
                q.Enqueue(2);
            }
            Assert.AreEqual(2, q.Count);
            Assert.AreEqual(1, q.Dequeue());
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmMemory.SystemTransactionsMode = NstmSystemTransactionsMode.EnlistOnBeginTransaction;
            using (System.Transactions.TransactionScope tx = new System.Transactions.TransactionScope())
            {
                q.Enqueue(3);
            }
            Assert.AreEqual(1, q.Count);
            Assert.AreEqual(2, q.Dequeue());
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            using (System.Transactions.TransactionScope tx = new System.Transactions.TransactionScope())
            {
                q.Enqueue(4);
                tx.Complete();
            }
            Assert.AreEqual(1, q.Count);
            Assert.AreEqual(4, q.Dequeue());
        }
    }
}

#endif