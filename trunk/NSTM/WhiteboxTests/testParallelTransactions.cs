#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

namespace NSTM.WhiteboxTests
{
    [TestFixture]
    public class testParallelTransactions
    {
        [Test]
        public void TestWithoutCollision()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                // simulate multithreading (i.e. object access with different tx) by using explicit tx
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsTrue(tx0.Commit(false));
            };
        }


        [Test]
        public void TestWithCollision()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsFalse(tx0.Commit(false));
            };

            Assert.AreEqual(1, o.Read());
        }


        [Test]
        public void TestValidateOnRead()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);

                Assert.IsTrue(tx1.Commit(false));

                try
                {
                    o.Read(NstmReadOption.ReadOnly, tx0);
                    Assert.Fail();
                }
                catch(Exception ex)
                {
                    Assert.IsInstanceOfType(typeof(NstmValidationFailedException), ex);
                }

                tx0.Rollback();
            };
        }


        [Test]
        public void TestValidateOnPassingRead()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.PassingReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);

                Assert.IsTrue(tx1.Commit(false));

                Assert.AreEqual(1, o.Read(NstmReadOption.PassingReadOnly, tx0));
                    // no validation for PassingReadOnly!

                tx0.Rollback();
            };
        }


        [Test]
        public void TestValidateOnCommit()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnRead))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);

                Assert.IsTrue(tx1.Commit(false));

                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));

                Assert.IsFalse(tx0.Commit(false));
            };
        }


        [Test]
        public void TestValidateOnCommitForReadCommitted()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.ReadCommitted, NstmTransactionCloneMode.CloneOnRead))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsFalse(tx0.Commit(false));
            };
        }


        [Test]
        public void TestWithPassingReadCollision()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.PassingReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsTrue(tx0.Commit(false));
            };

            Assert.AreEqual(1, o.Read());
        }


        [Test]
        public void TestWithSerializableOverwrite()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);
                o.Write(2, tx0);

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsFalse(tx0.Commit(false));
            };

            Assert.AreEqual(1, o.Read());
        }


        [Test]
        public void TestWithReadCommittedOverwrite()
        {
            NstmObject<int> o = (NstmObject<int>)NstmMemory.CreateObject<int>(0);

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.ReadCommitted, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx0));
                Assert.AreEqual(0, o.Read(NstmReadOption.ReadOnly, tx1));

                o.Write(1, tx1);
                o.Write(2, tx0);

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsTrue(tx0.Commit(false));
            };

            Assert.AreEqual(2, o.Read());
        }


        [Test]
        public void TestAgainstDeadlocksOnCommit()
        {
            List<NstmObject<int>> objects = new List<NstmObject<int>>();
            for (int i = 0; i < 3; i++)
                objects.Add((NstmObject<int>)NstmMemory.CreateObject<int>(i));

            using (INstmTransaction tx0 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            using (INstmTransaction tx1 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNew, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
            {
                // write to the objects in different order and with just an overlap so that each tx could commit some objects - but not others
                // that way it will lock some successfully and fail on others
                for (int i = 0; i < 2; i++)
                    objects[i].Write(i + 1, tx0);

                for (int i = 2; i >= 1; i--)
                    objects[i].Write(i * 10, tx1);

                Assert.IsTrue(tx1.Commit(false));
                Assert.IsFalse(tx0.Commit(false));
            };

            Assert.AreEqual(0, objects[0].Read());
            Assert.AreEqual(10, objects[1].Read());
            Assert.AreEqual(20, objects[2].Read());
        }
    }
}

#endif
