/* (c) 2007 by Ralf Westphal, Hamburg, Germany; www.ralfw.de, info@ralfw.de */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using NUnit.Framework;

using NSTM.Collections;

namespace test
{
    [TestFixture]
    public class testit
    {
        [Test]
        public void TestQueueMultithreaded()
        {
            const int NProducers = 5;
            const int NConsumers = 3;
            const int NEntriesPerProducer = 500;
            const int MsecProductionTime = 10;
            const int MsecConsumtionTime = 0;

            List<AutoResetEvent> ares = new List<AutoResetEvent>();
            
            NstmQueue<int> q = new NstmQueue<int>();
            int nProduced = 0;
            int nConsumed = 0;
            int nDequeueRetries = 0;

            // producers
            for (int i = 0; i < NProducers; i++)
            {
                ares.Add(new AutoResetEvent(false));
                ThreadPool.QueueUserWorkItem(
                    delegate(object state)
                    {
                        int thIndex = (int)state;
                        Console.WriteLine("prod {0} started...", thIndex);

                        int n = 0;
                        while(n<NEntriesPerProducer)
                        {
                            q.Enqueue(thIndex * 100000 + n);
                            n++;
                            lock (this) { nProduced++; }

                            Thread.Sleep(MsecProductionTime);
                        }

                        Console.WriteLine("  prod {0} finished: {1}, queue count: {2}, nProduced: {3}", thIndex, n, q.Count, nProduced);
                        ares[thIndex].Set();
                    },
                    i
                    );
            }

            // consumers
            for (int i = NProducers; i < (NProducers+NConsumers); i++)
            {
                ares.Add(new AutoResetEvent(false));
                ThreadPool.QueueUserWorkItem(
                    delegate(object state)
                    {
                        int thIndex = (int)state;
                        Console.WriteLine("cons {0} started...", thIndex);

                        int nConsumedByThread = 0;
                        while (true)
                        {
                            lock (this)
                            {
                                if (nProduced == NProducers * NEntriesPerProducer && nConsumed >= nProduced)
                                    break;
                            }

                            using (NSTM.INstmTransaction tx = NSTM.NstmMemory.BeginTransaction(
                                NSTM.NstmTransactionScopeOption.Required,
                                NSTM.NstmTransactionIsolationLevel.Serializable,
                                NSTM.NstmTransactionCloneMode.CloneOnWrite
                                ))
                            {
                                if (q.Count > 0)
                                {
                                    q.Dequeue();

                                    lock (this)
                                    {
                                        nConsumed++;
                                        nConsumedByThread++;
                                    }
                                }
                                else
                                {
                                    lock (this)
                                    {
                                        nDequeueRetries++;
                                    }
                                }
                            }

                            Thread.Sleep(MsecConsumtionTime);
                        }

                        Console.WriteLine("  cons {0} finished, consumed: {1}, nProduced: {2}, nConsumed: {3}", thIndex, nConsumedByThread, nProduced, nConsumed);

                        ares[thIndex].Set();
                    },
                    i
                    );
            }

            WaitHandle.WaitAll(ares.ToArray());

            Console.WriteLine("Dequeue retries: {0}", nDequeueRetries);

            Assert.AreEqual(nProduced, nConsumed);
        }


        [Test]
        public void TestQueuePerformance()
        {
            const int N = 750;
            testPerfOfBCLGenericQueue(N);
            testPerfOfNstmQueue(N);
            testPerfOfBCLGenericQueue(N);
            testPerfOfNstmQueue(N);
            testPerfOfBCLGenericQueue(N);
            testPerfOfNstmQueue(N);
        }

        private void testPerfOfBCLGenericQueue(int N)
        {
            Queue<int> q = new Queue<int>();
            for (int i = 0; i < N; i++)
                lock (q)
                {
                    q.Enqueue(i);
                }
        }

        private void testPerfOfNstmQueue(int N)
        {
            NstmQueue<int> q = new NstmQueue<int>();
            for (int i = 0; i < N; i++)
                q.Enqueue(i);
        }
    }


    //[TestFixture]
    //public class TestAttributesExtern
    //{
    //    [Test]
    //    public void TestTransactional()
    //    {
    //        NSTM.INstmObject<int> o = NSTM.NstmMemory.CreateObject<int>(1);
    //        try
    //        {
    //            DoSth(o, true);
    //        }
    //        catch (ApplicationException ex)
    //        {}
    //        Assert.AreEqual(1, o.Read());
    //    }

    //    [NSTM.NstmTransactional]
    //    private void DoSth(NSTM.INstmObject<int> o, bool fail)
    //    {
    //        o.Write(o.Read() * 2);
    //        if (fail) throw new ApplicationException("tx failed!");
    //    }
    //}
}
