using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using NSTM;

namespace NSTM.BlackboxTests
{
    [TestFixture]
    public class testsExecAtomically
    {
        [Test]
        public void TestSimple()
        {
            NstmTransactional<int> iTx = 0;

            NstmMemory.ExecuteAtomically(
                delegate
                {
                    iTx.Value = 1;
                }
                );

            Assert.AreEqual(1, iTx.Value);
        }


        [Test]
        public void TestSimpleWithAppException()
        {
            NstmTransactional<int> iTx = 0;

            try
            {
                NstmMemory.ExecuteAtomically(
                    delegate
                    {
                        iTx.Value = 1;
                        throw new ApplicationException("test failed!");
                    }
                    );
            }
            catch (ApplicationException ex)
            {
                Assert.IsTrue(ex.Message == "test failed!");
            }
            catch
            {
                Assert.Fail();
            }

            Assert.AreEqual(0, iTx.Value);
        }


        [Test]
        public void TestWithRetry()
        {
            System.Threading.AutoResetEvent areBg = new System.Threading.AutoResetEvent(false);
            System.Threading.AutoResetEvent areFg = new System.Threading.AutoResetEvent(false);

            NstmTransactional<int> iTx = 0;

            System.Threading.ThreadPool.QueueUserWorkItem(
                delegate
                {
                    areBg.WaitOne();
                    iTx.Value = 99;
                    areFg.Set();
                }
                );

            int iTrial = 0;
            NstmMemory.ExecuteAtomically(
                true,
                delegate
                {
                    iTrial++;

                    int v = iTx.Value;
                    if (iTrial == 1)
                    {
                        areBg.Set();
                        areFg.WaitOne();
                    }

                    v = iTx.Value; // this should fail only on iTrial=1

                    iTx.Value = iTx.Value + 1;
                }
                );

            Assert.AreEqual(2, iTrial);
            Assert.AreEqual(100, iTx.Value);
        }


        [Test]
        public void TestWithMaxRetriesFailure()
        {
            System.Threading.AutoResetEvent areBg = new System.Threading.AutoResetEvent(false);
            System.Threading.AutoResetEvent areFg = new System.Threading.AutoResetEvent(false);

            NstmTransactional<int> iTx = 0;

            System.Threading.ThreadPool.QueueUserWorkItem(
                delegate
                {
                    areBg.WaitOne();
                    iTx.Value = 99;
                    areFg.Set();

                    areBg.WaitOne();
                    iTx.Value = 100;
                    areFg.Set();

                    areBg.WaitOne();
                    iTx.Value = 101;
                    areFg.Set();
                }
                );

            int iTrial = 0;

            try
            {
                NstmMemory.ExecuteAtomically(
                    NstmTransactionScopeOption.Required,
                    NstmTransactionIsolationLevel.Serializable,
                    NstmTransactionCloneMode.CloneOnWrite,
                    2,
                    System.Threading.Timeout.Infinite,
                    System.Threading.Timeout.Infinite,
                    delegate
                    {
                        iTrial++;

                        int v = iTx.Value;
                        areBg.Set();
                        areFg.WaitOne();

                        v = iTx.Value; // this should always fail
                    }
                    );

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(NstmRetryFailedException), ex);
            }

            Assert.AreEqual(3, iTrial);
        }


        [Test]
        public void TestWithMaxProcessingTimeFailure()
        {
            System.Threading.AutoResetEvent areBg = new System.Threading.AutoResetEvent(false);
            System.Threading.AutoResetEvent areFg = new System.Threading.AutoResetEvent(false);

            NstmTransactional<int> iTx = 0;

            System.Threading.ThreadPool.QueueUserWorkItem(
                delegate
                {
                    areBg.WaitOne();
                    iTx.Value = 99;
                    areFg.Set();

                    areBg.WaitOne();
                    iTx.Value = 100;
                    areFg.Set();

                    areBg.WaitOne();
                    iTx.Value = 101;
                    areFg.Set();
                }
                );

            int iTrial = 0;

            try
            {
                NstmMemory.ExecuteAtomically(
                    NstmTransactionScopeOption.Required,
                    NstmTransactionIsolationLevel.Serializable,
                    NstmTransactionCloneMode.CloneOnWrite,
                    int.MaxValue,
                    System.Threading.Timeout.Infinite,
                    550,
                    delegate
                    {
                        iTrial++;

                        int v = iTx.Value;
                        areBg.Set();
                        areFg.WaitOne();

                        Console.WriteLine(iTrial);
                        System.Threading.Thread.Sleep(300);
                        Console.WriteLine("---");

                        v = iTx.Value; // this should always fail
                    }
                    );

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(NstmRetryFailedException), ex);
            }
        }


        [Test]
        public void TestDelayAndFailure()
        {
            System.Threading.AutoResetEvent areBg = new System.Threading.AutoResetEvent(false);
            System.Threading.AutoResetEvent areFg = new System.Threading.AutoResetEvent(false);

            NstmTransactional<int> iTx = 0;

            System.Threading.ThreadPool.QueueUserWorkItem(
                delegate
                {
                    areBg.WaitOne();
                    iTx.Value = 99;
                    areFg.Set();

                    areBg.WaitOne();
                    iTx.Value = 100;
                    areFg.Set();

                    areBg.WaitOne();
                    iTx.Value = 101;
                    areFg.Set();
                }
                );

            int iTrial = 0;

            try
            {
                NstmMemory.ExecuteAtomically(
                    NstmTransactionScopeOption.Required,
                    NstmTransactionIsolationLevel.Serializable,
                    NstmTransactionCloneMode.CloneOnWrite,
                    int.MaxValue,
                    250,
                    550,
                    delegate
                    {
                        iTrial++;

                        int v = iTx.Value;
                        areBg.Set();
                        areFg.WaitOne();

                        Console.WriteLine(iTrial);

                        v = iTx.Value; // this should always fail
                    }
                    );

                Assert.Fail();
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(typeof(NstmRetryFailedException), ex);
            }
        }
    
    
    }
}
