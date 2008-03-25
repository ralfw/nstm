using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NSTM;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testNstmTransactional_of_T
    {
        [Test]
        public void TestScalar()
        {
            NstmTransactional<int> i = 1;
            NstmTransactional<double> f = 3.14;
            NstmTransactional<double> f2 = f;
            Assert.AreEqual(1, i.Value);
            Assert.AreEqual(3.14, f.Value);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                i.Value = 2;
                f = 9.81; // create new NstmTransactional<> object behind the scene
                tx.Rollback();
            }

            Assert.AreEqual(1, i.Value);
            Assert.AreEqual(0, f.Value);
            Assert.AreEqual(3.14, f2.Value);

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                i.Value = 2;
                f = 9.81; // create another new NstmTransactional<> object behind the scene
                tx.Commit();
            }

            Assert.AreEqual(2, i.Value);
            Assert.AreEqual(9.81, f.Value);
            Assert.AreEqual(3.14, f2.Value);
        }


        [Test]
        public void TestValueType()
        {
            NstmTransactional<MyValues0> v = new MyValues0(1, "hello");

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                v.Value = new MyValues0(2, "world!");
            }

            Assert.AreEqual(1, v.Value.i);
            Assert.AreEqual("hello", v.Value.S);


            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                v.Value = new MyValues0(2, "world!");
                tx.Commit();
            }

            Assert.AreEqual(2, v.Value.i);
            Assert.AreEqual("world!", v.Value.S);
        }

        
        [Test]
        public void TestClass()
        {
            try
            {
                NstmTransactional<MyClass1> v = new MyClass1();
                Assert.Fail();
            }
            catch (TypeInitializationException ex)
            {
                Assert.IsTrue(ex.InnerException.Message.IndexOf("Invalid type") >= 0);
            }
        }


        [Test]
        public void TestString()
        {
            NstmTransactional<string> v = "hello";

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                v.Value = "world!";
            }

            Assert.AreEqual("hello", v.Value);


            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                v.Value = "world!";
                tx.Commit();
            }
            Assert.AreEqual("world!", v.Value);
        }

        [Test]
        public void TestNewAssignmentInTx()
        {
            NstmTransactional<int> iTx = 99;
            NstmTransactional<int> iTxBackup = iTx;
            Assert.AreEqual(iTxBackup.GetHashCode(), iTx.GetHashCode());

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                iTx = 99;
                tx.Rollback();
            }

            Assert.AreNotEqual(iTxBackup.GetHashCode(), iTx.GetHashCode());
            Assert.AreEqual(0, iTx.Value);
        }
    }


    public struct MyValues0
    {
        public int i;
        private string s;

        public MyValues0(int i, string s)
        {
            this.i = i;
            this.s = s;
        }

        public string S
        {
            get
            {
                return this.s;
            }
            set
            {
                this.s = value;
            }
        }
    }

    public class MyClass1
    {
        public int i;
    }
}
