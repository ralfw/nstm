#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NSTM;

namespace NSTM.WhiteboxTests
{    
    // unlock acquired locks - konkurrierende tx

    [TestFixture]
    public class testNstmTransactionBasics
    {
        [Test]
        public void TestTxActivityMode()
        {
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                Assert.AreEqual(NstmTransactionActivityMode.Active, tx.ActivityMode);

                tx.Commit();

                Assert.AreEqual(NstmTransactionActivityMode.Committed, tx.ActivityMode);
            }

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
                Assert.AreEqual(NstmTransactionActivityMode.Active, tx.ActivityMode);

                tx.Rollback();

                Assert.AreEqual(NstmTransactionActivityMode.Aborted, tx.ActivityMode);
            }
        }


        [Test]
        public void TestTxLog()
        {
            INstmObject<string> p = NstmMemory.CreateObject<string>();
            p.Write("hello");

            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(0, tx.LogCount);

                INstmObject<int> o = NstmMemory.CreateObject<int>();
                Assert.AreEqual(0, tx.LogCount);

                o = NstmMemory.CreateObject<int>(1);
                Assert.AreEqual(1, tx.LogCount);

                Assert.AreEqual("hello", p.Read(NstmReadOption.ReadOnly));
                Assert.AreEqual(2, tx.LogCount);

                Assert.AreEqual(1, o.Read(NstmReadOption.ReadOnly));
                Assert.AreEqual(2, tx.LogCount);
            }
        }


        [Test]
        public void TestCommitWithBoolReturn()
        {
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                Assert.IsTrue(tx.Commit(false));
            }
        }


        [Test]
        public void TestReadWriteCommit()
        {
            List<INstmObject<int>> l;
            l = new List<INstmObject<int>>();

            l.Add(NstmMemory.CreateObject<int>(1)); // create outside tx and write
            l.Add(NstmMemory.CreateObject<int>());  // create outside tx
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                l.Add(NstmMemory.CreateObject<int>(3)); // create inside tx and write

                l[0].Write(10);
                l[1].Write(20);
                l[2].Write(30);

                tx.Commit();
            }
            Assert.AreEqual(10, l[0].Read());
            Assert.AreEqual(20, l[1].Read());
            Assert.AreEqual(30, l[2].Read());
        }


        [Test]
        public void TestReadWriteRollback()
        {
            List<INstmObject<int>> l;
            l = new List<INstmObject<int>>();

            l.Add(NstmMemory.CreateObject<int>(1));
            l.Add(NstmMemory.CreateObject<int>());
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                l.Add(NstmMemory.CreateObject<int>(3));

                l[0].Write(10);
                l[1].Write(20);
                l[2].Write(30);

                tx.Rollback();
            }
            Assert.AreEqual(1, l[0].Read());
            Assert.AreEqual(0, l[1].Read());
            Assert.AreEqual(0, l[2].Read()); // object exists outside tx, but value from inside tx has been rolled back

            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                l[0].Write(100);
            } // default at end of using() is to rollback
            Assert.AreEqual(1, l[0].Read());
        }


        [Test]
        public void TestNestedTxCommit()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                o.Write(2);
                Assert.AreEqual(2, o.Read(NstmReadOption.ReadOnly));
                using (NstmTransaction tx2 = (NstmTransaction)NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNested, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
                {
                    o.Write(3);
                    Assert.AreEqual(3, o.Read(NstmReadOption.ReadOnly));
                    tx2.Commit();
                }
                Assert.AreEqual(3, o.Read(NstmReadOption.ReadOnly));
                tx.Commit();
            }
            Assert.AreEqual(3, o.Read(NstmReadOption.ReadOnly));
        }


        [Test]
        public void TestNestedTxRollback()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                o.Write(2);
                Assert.AreEqual(2, o.Read(NstmReadOption.ReadOnly));
                using (NstmTransaction tx2 = (NstmTransaction)NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNested, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
                {
                    o.Write(3);
                    Assert.AreEqual(3, o.Read(NstmReadOption.ReadOnly));
                    tx2.Commit();
                }
                Assert.AreEqual(3, o.Read(NstmReadOption.ReadOnly));
                tx.Rollback();
            }
            Assert.AreEqual(1, o.Read(NstmReadOption.ReadOnly));
        }


        [Test]
        public void TestCommitReadOnlyObjects()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            INstmObject<int> p = NstmMemory.CreateObject<int>(2);
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, o.Read(NstmReadOption.ReadOnly));
                Assert.AreEqual(2, p.Read(NstmReadOption.PassingReadOnly));
                tx.Commit();
            }
        }


        [Test]
        public void TestMultipleCommitRollback()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                o.Write(2);
                tx.Commit();
                tx.Commit();
            }
            Assert.AreEqual(2, o.Read());

            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                o.Write(3);
                tx.Commit();
                tx.Rollback();
            }
            Assert.AreEqual(3, o.Read());

            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                o.Write(4);
                tx.Rollback();
                tx.Commit();
            }
            Assert.AreEqual(3, o.Read());

            using (NstmTransaction tx = (NstmTransaction)NstmMemory.BeginTransaction())
            {
                o.Write(5);
                tx.Rollback();
                tx.Rollback();
            }
            Assert.AreEqual(3, o.Read());
        }
    }
}

#endif