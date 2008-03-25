using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using NSTM;

namespace NSTM.BlackboxTests.ErrorReproduction
{
    [TestFixture]
    public class testIssue1
    {
        [Test]
        public void testInconsistencyDueToGetHashCode()
        {
            int nmax = 100000;
            INstmObject<int>[] array = new INstmObject<int>[nmax];

            using (INstmTransaction tx = NstmMemory.BeginTransaction())
            {
                for (int n = 0; n < nmax; ++n)
                {
                    array[n] = NstmMemory.CreateObject<int>(n);
                    Assert.AreEqual(n, array[n].Read());
                }
                tx.Commit();
            }
            for (int n = 0; n < nmax; ++n)
            {
                Assert.AreEqual(n, array[n].Read());
            }
        }
    }
}
