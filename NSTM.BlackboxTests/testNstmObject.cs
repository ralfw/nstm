using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NSTM;

namespace NSTM.BlackboxTests
{
    // internal value/version set


    [TestFixture]
    public class testNstmObject
    {
        [Test]
        public void TestNonTxReadWrite()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>();
            o.Write(1);
            Assert.AreEqual(1, o.Read(NstmReadOption.ReadOnly));
        }


        [Test]
        public void TestVersion()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>();
            Assert.AreEqual(0, o.Version);
            o.Write(1);
            Assert.AreEqual(1, o.Version);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                o.Write(2);
                Assert.AreEqual(1, o.Version);
                tx.Commit();
            }
            Assert.AreEqual(2, o.Version);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                o.Write(3);
                Assert.AreEqual(2, o.Version);
            }
            Assert.AreEqual(2, o.Version);
        }


        [Test]
        public void TestReadMultiple()
        {
            INstmObject<int> o = NstmMemory.CreateObject<int>(1);
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual(1, o.Read(NstmReadOption.ReadOnly));
                Assert.AreEqual(1, o.Read(NstmReadOption.ReadOnly));
            }
        }


        [Test]
        public void TestCloneScalarValues()
        {
            INstmObject<string> oString = NstmMemory.CreateObject<string>("hello");
            INstmObject<double> oDouble = NstmMemory.CreateObject<double>(3.14);
            INstmObject<DateTime> oDateTime = NstmMemory.CreateObject<DateTime>(new DateTime(2007, 6, 27));

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                Assert.AreEqual("hello", oString.Read(NstmReadOption.ReadWrite));
                Assert.AreEqual(3.14, oDouble.Read(NstmReadOption.ReadWrite));
                Assert.AreEqual(new DateTime(2007, 6, 27), oDateTime.Read(NstmReadOption.ReadWrite));
            }
        }


        [Test]
        public void TestCloneCustomValueType()
        {
            MyValueType vt = new MyValueType();
            vt.i = 1;
            vt.s = "hello";
            INstmObject<MyValueType> oVT = NstmMemory.CreateObject<MyValueType>(vt);
            
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                MyValueType vt2 = oVT.Read(NstmReadOption.ReadWrite);
                Assert.AreEqual(vt.i, vt2.i);
                Assert.AreEqual(vt.s, vt2.s);
            }
        }


        [Test]
        public void TestCloneClass()
        {
            MyCloneableClass c = new MyCloneableClass();
            c.i = 1;
            c.s = "hello";
            INstmObject<MyCloneableClass> oC = NstmMemory.CreateObject<MyCloneableClass>(c);
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                MyCloneableClass c2 = oC.Read(NstmReadOption.ReadWrite);
                Assert.AreNotEqual(c, c2);
                Assert.AreEqual(c.i, c2.i);
                Assert.AreEqual(c.s, c2.s);
            }

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                MyCloneableClass c2 = oC.Read(); // the default read option is "ReadWrite"
                Assert.AreNotEqual(c, c2);
            }
        }


        [Test]
        public void TestNonCloneableClass()
        {
            try
            {
                INstmObject<MyNonCloneableClass> c;
                c = NstmMemory.CreateObject<MyNonCloneableClass>();
                Assert.Fail();
            }
            catch (TypeInitializationException ex)
            {
                Assert.AreEqual(typeof(InvalidCastException), ex.InnerException.GetType());
            }
            catch
            {
                Assert.Fail();
            }
        }
    }

    internal struct MyValueType
    {
        public int i;
        public string s;
    }

    internal class MyCloneableClass : ICloneable
    {
        public int i;
        public string s;

        #region ICloneable Members

        public object Clone()
        {
            MyCloneableClass c = new MyCloneableClass();
            c.i = this.i;
            c.s = this.s;
            return c;
        }

        #endregion
    }

    internal class MyNonCloneableClass
    {
        public int i=0;
    }
}
