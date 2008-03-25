/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

namespace NSTM.Collections.BlackboxTests
{
    [TestFixture]
    public class testNstmList
    {
        [Test]
        public void TestAddAndCount()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmList<int> l = new NstmList<int>();
            Assert.AreEqual(0, l.Count);

            l.Add(1);
            Assert.AreEqual(1, l.Count);
            l.Add(2);
            Assert.AreEqual(2, l.Count);
            l.Add(3);
            Assert.AreEqual(3, l.Count);
        }


        [Test]
        public void TestGet()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmList<int> l = new NstmList<int>();
            l.Add(1);
            l.Add(2);
            l.Add(3);

            Assert.AreEqual(3, l[2]);
            Assert.AreEqual(2, l[1]);
            Assert.AreEqual(1, l[0]);

            try
            {
                Assert.AreEqual(0, l[3]);
            }
            catch (IndexOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("out of") >= 0);
            }

            try
            {
                Assert.AreEqual(0, l[-1]);
            }
            catch (IndexOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("out of") >= 0);
            }
        }


        [Test]
        public void TestSet()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmList<int> l = new NstmList<int>();
            l.Add(1);

            l[0] = 2;
            Assert.AreEqual(2, l[0]);

            try
            {
                l[1] = 3;
            }
            catch (IndexOutOfRangeException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("out of") >= 0);
            }
        }


        [Test]
        public void TestRemoveAt()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmList<int> l = new NstmList<int>();
            l.Add(1);
            l.Add(2);
            l.Add(3);
            Assert.AreEqual(1, l[0]);

            Assert.IsTrue(l.RemoveAt(1));
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(1, l[0]);
            Assert.AreEqual(3, l[1]);
            Assert.IsTrue(l.RemoveAt(1));
            Assert.AreEqual(1, l.Count);
            Assert.IsTrue(l.RemoveAt(0));
            Assert.AreEqual(0, l.Count);
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
            Assert.IsFalse(l.RemoveAt(1));
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);
            //l.Add(10);
            //l.Add(11);
            //Assert.IsTrue(l.RemoveAt(0));
        }


        [Test]
        public void TestInsertBefore()
        {
            Assert.AreEqual(0, NstmMemory.ActiveTransactionCount);

            NstmList<int> l = new NstmList<int>();

            l.InsertBefore(0, 1); // 1
            Assert.AreEqual(1, l.Count);
            Assert.AreEqual(1, l[0]);

            l.InsertBefore(0, 2); // 2, 1
            Assert.AreEqual(2, l.Count);
            Assert.AreEqual(2, l[0]);
            Assert.AreEqual(1, l[1]);

            l.InsertBefore(100, 3); // 2, 1, 3
            Assert.AreEqual(3, l.Count);
            Assert.AreEqual(3, l[2]);
            Assert.AreEqual(2, l[0]);
            Assert.AreEqual(1, l[1]);

            l.InsertBefore(1, 4); // 2, 4, 1, 3
            Assert.AreEqual(4, l.Count);
            Assert.AreEqual(4, l[1]);
            Assert.AreEqual(2, l[0]);
            Assert.AreEqual(1, l[2]);
        }
    }
}

#endif