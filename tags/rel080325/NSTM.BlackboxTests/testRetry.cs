using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using System.Threading;

using NSTM;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testRetry
    {
        [Test]
        public void TestWithoutTx()
        {
            NstmTransactional<int> iTx = 0;

            AutoResetEvent are = new AutoResetEvent(false);
            int nRetries = 0;

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    are.WaitOne();
                    Thread.Sleep(500); // wait a little until Retry() has been executed
                    iTx.Value = 1;
                });

            NstmMemory.ExecuteAtomically(
                delegate
                {
                    nRetries++;
                    if (iTx.Value == 0)
                    {
                        are.Set();  // inform thread it can modify the value
                        NstmMemory.Retry();
                    }
                    iTx.Value++;
                }
                );

            Assert.AreEqual(2, nRetries);
            Assert.AreEqual(2, iTx.Value);
        }


        [Test]
        public void TestWithTx()
        {
            NstmTransactional<int> iTx = 0;

            AutoResetEvent are = new AutoResetEvent(false);
            int nRetries = 0;

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    are.WaitOne();
                    Thread.Sleep(200); // wait a little until Retry() has been executed
                    // wrap change into a tx
                    NstmMemory.ExecuteAtomically(
                        delegate
                        {
                            iTx.Value = 1;
                        });
                });

            NstmMemory.ExecuteAtomically(
                delegate
                {
                    nRetries++;
                    if (iTx.Value == 0)
                    {
                        are.Set();  // inform thread it can modify the value
                        NstmMemory.Retry();
                    }
                    iTx.Value++;
                });

            Assert.AreEqual(2, nRetries);
            Assert.AreEqual(2, iTx.Value);
        }


        [Test]
        public void TestWithTimeout()
        {
            NstmTransactional<int> iTx = 0;

            AutoResetEvent are = new AutoResetEvent(false);
            int nRetries = 0;

            ThreadPool.QueueUserWorkItem(
                delegate
                {
                    are.WaitOne();
                    Thread.Sleep(300); // wait a little until Retry() has been executed
                });

            try
            {
                NstmMemory.ExecuteAtomically(
                    delegate
                    {
                        nRetries++;
                        if (iTx.Value == 0)
                        {
                            are.Set();  // inform thread it can modify the value
                            NstmMemory.Retry(10);
                        }
                        iTx.Value++;
                    }
                    );
                Assert.Fail("no exception was thrown!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.GetType().Name);
                Assert.IsNotNull(ex as NstmRetryFailedException, "unexpected exception type");
            }

            Assert.AreEqual(1, nRetries);
            Assert.AreEqual(0, iTx.Value);
        }
    }
}
