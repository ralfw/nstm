using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NSTM;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testErrorReproduction
    {
        [Test]
        public void ReproduceEmptyTempvalue()
        {
            // s. NstmList.RemoveAt(): After commit the value of some objects is null because the logentry tempvalue seems to not have been set despite ReadWrite access

            INstmObject<MyInt> o = NstmMemory.CreateObject<MyInt>();
            MyInt i = new MyInt();
            i.value = 1;
            o.Write(i);

            using (INstmTransaction tx = NstmMemory.BeginTransaction(
                                                    NstmTransactionScopeOption.RequiresNested,
                                                    NstmTransactionIsolationLevel.ReadCommitted,
                                                    NstmTransactionCloneMode.CloneOnWrite))
            {
                MyInt j = o.Read(NstmReadOption.PassingReadOnly);
                Assert.AreEqual(i, j);

                j = o.Read(NstmReadOption.ReadWrite);
                Assert.AreNotEqual(i, j);
                j.value = 2;

                tx.Commit();
            }

            Assert.AreEqual(2, o.Read().value);
        }

        private class MyInt : ICloneable
        {
            public int value;

            #region ICloneable Members

            public object Clone()
            {
                MyInt o = new MyInt();
                o.value = this.value;
                return o;
            }

            #endregion
        }
    }

    //// does PostSharp weave the field access aspect within the compound aspect correctly with generic type fields?
    //[NstmTransactional]
    //public class MyGenericAggregator
    //{
    //    private List<int> myVar;

    //    public List<int> MyProperty
    //    {
    //        get { return myVar; }
    //        set { myVar = value; }
    //    }
    //}
}
