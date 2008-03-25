using System;
using System.Collections.Generic;
using System.Text;

using System.Threading;
using NUnit.Framework;
using NSTM;
using NSTM.Collections;

namespace NSTM.Collections.BlackboxTests
{
    [TestFixture]
    public class testNstmStack
    {
        [Test]
        public void testPushCount()
        {
            NstmStack<int> s = new NstmStack<int>();
            Assert.AreEqual(0, s.Count);

            s.Push(1);
            Assert.AreEqual(1, s.Count);
            s.Push(2);
            Assert.AreEqual(2, s.Count);
            s.Push(3);
            Assert.AreEqual(3, s.Count);
        }

        [Test]
        public void testPopCount()
        {
            NstmStack<int> s = new NstmStack<int>();
            s.Push(1);
            Assert.AreEqual(1, s.Pop());
            Assert.AreEqual(0, s.Count);

            s.Push(1);
            s.Push(2);
            Assert.AreEqual(2, s.Pop());
            Assert.AreEqual(1, s.Count);
            Assert.AreEqual(1, s.Pop());
            Assert.AreEqual(0, s.Count);

            try
            {
                s.Pop();
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("empty") >= 0);
            }
        }

        [Test]
        public void testPeekCount()
        {
            NstmStack<int> s = new NstmStack<int>();
            s.Push(1);
            Assert.AreEqual(1, s.Peek());
            Assert.AreEqual(1, s.Count);

            s.Pop();
            try
            {
                s.Peek();
                Assert.Fail();
            }
            catch (InvalidOperationException ex)
            {
                Assert.IsTrue(ex.Message.IndexOf("empty") >= 0);
            }
        }

        [Test]
        public void testTx()
        {
            NstmStack<int> s = new NstmStack<int>();
            s.Push(1);
            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                s.Push(2);
                tx.Commit();
            }
            Assert.AreEqual(2, s.Count);
            Assert.AreEqual(2, s.Peek());

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                s.Pop();
                Assert.AreEqual(1, s.Peek());
                Assert.AreEqual(1, s.Count);
                s.Push(3);
                Assert.AreEqual(3, s.Peek());
                Assert.AreEqual(2, s.Count);

                tx.Rollback();
            }
            Assert.AreEqual(2, s.Count);
            Assert.AreEqual(2, s.Peek());
        }


        [Test]
        public void testParallel()
        {
            const int N = 500;

            NstmStack<int> s = new NstmStack<int>();

            AutoResetEvent areStartProduction = new AutoResetEvent(false);
            
            // thread producing values on the stack
            int nWriteTrials = 0;
            Thread thp = new Thread(
                delegate()
                {
                    areStartProduction.WaitOne(); // wait until consumer is ready
                    for (int i = 0; i < N; i++)
                    {
                        Console.WriteLine("push: " + i.ToString());

                        NstmMemory.ExecuteAtomically(
                            NstmTransactionScopeOption.RequiresNew,
                            NstmTransactionIsolationLevel.Serializable,
                            NstmTransactionCloneMode.CloneOnRead,
                            true,
                            delegate
                            {
                                s.Push(i);
                                nWriteTrials++;
                            }
                        );
                    }
                }
                );
            thp.IsBackground = true;
            thp.Start();

            // thread consuming values from the stack
            int nRead = 0;
            int nReadTrials = 0;
            int nReadManualTrials = 0;
            Thread thc = new Thread(
                delegate()
                {
                    areStartProduction.Set();  // kick-off producer
                    while (true)
                    {
                        NstmMemory.ExecuteAtomically(
                            NstmTransactionScopeOption.RequiresNew,
                            NstmTransactionIsolationLevel.Serializable,
                            NstmTransactionCloneMode.CloneOnRead,
                            true,
                            delegate
                            {
                                nReadTrials++;

                                if (s.Count == 0)
                                {
                                    nReadManualTrials++;
                                    NstmMemory.Retry();
                                }

                                nRead++;
                                Console.WriteLine("pop: {0} - {1}", s.Pop(), nRead);
                            }
                        );

                        //bool popped = false;
                        //NstmMemory.ExecuteAtomically(
                        //    true,
                        //    delegate
                        //    {
                        //        popped = false;

                        //        if (s.Count > 0)
                        //        {
                        //            nReadTrials++;

                        //            Console.WriteLine("pop: " + s.Pop());
                        //            popped = true;
                        //        }
                        //    }
                        //);

                        //if (popped) nRead++;

                        if (nRead == N) break;
                    }
                }
                );
            thc.IsBackground = true;
            thc.Start();

            thp.Join();
            thc.Join();

            Console.WriteLine("--n write trials: {0}", nWriteTrials);
            Console.WriteLine("--n read trials: {0}", nReadTrials);
            Console.WriteLine("--n read manual trials: {0}", nReadManualTrials);

            Console.WriteLine("--s.count: {0}", s.Count);
            NstmStack<int>.Entry e = s.Top;
            while (e != null)
            {
                Console.WriteLine("----{0}", e.Value);
                e = e.Next;
            }

            Assert.AreEqual(N, nRead);
            Assert.AreEqual(0, s.Count);
        }
    }
}
