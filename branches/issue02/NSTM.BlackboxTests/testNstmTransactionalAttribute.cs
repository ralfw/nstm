using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NSTM;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testNstmTransactionalAttribute
    {
        [Test]
        public void TestTransactionalTypeNoTx()
        {
            MyClass0 o = new MyClass0();
            INstmVersioned vo = (INstmVersioned)(object)o;
            Assert.AreEqual(0, vo.Version);

            o.i = 1;
            Assert.AreEqual(1, o.i);
            Assert.AreEqual(1, vo.Version);
        }


        [Test]
        public void TestTransactionalTypeWithTxRollback()
        {
            MyClass0 o = new MyClass0();
            o.i = 1;
            o.S = "hello";
            INstmVersioned vo = (INstmVersioned)(object)o;
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(2, vo.Version);
                o.i = 2;
                o.S = "world!";
                Assert.AreEqual(2, o.i);
                Assert.AreEqual(2, vo.Version);
            }
            Assert.AreEqual(2, vo.Version);
            Assert.AreEqual(1, o.i);
            Assert.AreEqual("hello", o.S);
        }


        [Test]
        public void TestTransactionalTypeWithTxCommit()
        {
            MyClass0 o = new MyClass0();
            o.i = 1;
            o.S = "hello";
            INstmVersioned vo = (INstmVersioned)(object)o;
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(2, vo.Version);
                o.i = 2;
                o.S = "world!";
                Assert.AreEqual(2, o.i);
                Assert.AreEqual(2, vo.Version);

                tx.Commit();
            }
            Assert.AreEqual(3, vo.Version);
            Assert.AreEqual(2, o.i);
            Assert.AreEqual("world!", o.S);
        }


        [Test]
        public void TestTransactionalTypNested()
        {
            MyClass0 o = new MyClass0();
            o.i = 1;
            o.S = "hello";
            INstmVersioned vo = (INstmVersioned)(object)o;

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                o.i = 10;

                using (INstmTransaction tx2 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNested, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
                {
                    Assert.AreEqual(10, o.i);
                    o.i = 2;
                    o.S = "world!";
                    Assert.AreEqual(2, o.i);

                    tx2.Rollback();
                }

                Assert.AreEqual(10, o.i);

                tx.Commit();
            }

            Assert.AreEqual(10, o.i);
            Assert.AreEqual("hello", o.S);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                o.i = 11;

                using (INstmTransaction tx2 = NstmMemory.BeginTransaction(NstmTransactionScopeOption.RequiresNested, NstmTransactionIsolationLevel.Serializable, NstmTransactionCloneMode.CloneOnWrite))
                {
                    Assert.AreEqual(11, o.i);
                    o.i = 12;
                    o.S = "world!";
                    Assert.AreEqual(12, o.i);

                    tx2.Commit();
                }

                Assert.AreEqual(12, o.i);

                tx.Commit();
            }
            Assert.AreEqual(12, o.i);
            Assert.AreEqual("world!", o.S);
        }


        [NstmTransactional]
        public class MyClass0
        {
            internal int i;
            private string s;

            public string S
            {
                get { return this.s; }
                set { this.s = value; }
            }
	
        }
    }
}
