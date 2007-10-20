using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NSTM;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testNstmAtomicAttribute
    {
        [Test]
        public void TestTxMethod()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            TxDefault(o, false);
            Assert.AreEqual(2, o.Read());

            try
            {
                TxDefault(o, true);
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.AreEqual("tx failed!", ex.Message);
                Assert.AreEqual(2, o.Read());
            }
        }

        [NstmAtomic]
        private void TxDefault(INstmObject<int> o, bool fail)
        {
            Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);

            o.Write(o.Read() * 2);
            if (fail) throw new ApplicationException("tx failed!");
        }


        [Test]
        public void TestTxNested()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            TxLevel0(o, false);
            Assert.AreEqual(4, o.Read());

            try
            {
                TxLevel0(o, true);
                Assert.Fail();
            }
            catch (ApplicationException ex)
            {
                Assert.AreEqual("tx failed!", ex.Message);
                Assert.AreEqual(4, o.Read());
            }
        }

        [NstmAtomic(NstmTransactionScopeOption.RequiresNested, 
                    NstmTransactionIsolationLevel.Serializable, 
                    NstmTransactionCloneMode.CloneOnWrite)]
        private void TxLevel0(INstmObject<int> o, bool fail)
        {
            Assert.AreEqual(1, NstmMemory.ActiveTransactionCount);
            Assert.IsFalse(NstmMemory.Current.IsNested);
            o.Write(o.Read() * 2);
            TxLevel1(o, fail);
        }

        [NstmAtomic(NstmTransactionScopeOption.RequiresNested,
                               NstmTransactionIsolationLevel.Serializable,
                               NstmTransactionCloneMode.CloneOnWrite)]
        private void TxLevel1(INstmObject<int> o, bool fail)
        {
            Assert.AreEqual(2, NstmMemory.ActiveTransactionCount);
            Assert.IsTrue(NstmMemory.Current.IsNested);
            o.Write(o.Read() * 2);
            if (fail) throw new ApplicationException("tx failed!");
        }


        [Test]
        public void TestNestedRequired()
        {
            // only create one tx but call method recursively.
            // for each level the aspect will try to close a tx
            // but this must only happen for tx that were created
            // upon entry of method.
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
            TxMultilevel(1);
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
        }

        [NstmAtomic]
        private void TxMultilevel(int level)
        {
            if (level > 0) TxMultilevel(level - 1);
        }
    }
}
