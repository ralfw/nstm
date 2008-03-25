#if DEBUG

using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using System.Threading;
using System.Collections;

namespace NSTM.Infrastructure
{
    [TestFixture]
    public partial class RetryTriggerList
    {
        [SetUp]
        public void clearInstance()
        {
            // need to reset the one instance on which all tests are run before every test
            this.Clear();
        }


        [Test]
        public void testRegister()
        {
            object o1 = 1;
            object o2 = 2;

            AutoResetEvent are1 = new AutoResetEvent(false);
            AutoResetEvent are2 = new AutoResetEvent(false);

            this.RegisterRetry(o1, are1);
            Assert.AreEqual(1, this.triggers.Count);
            Assert.IsTrue(this.triggers.ContainsKey(o1));
            Assert.IsTrue(((Hashtable)this.triggers[o1]).ContainsKey(are1));

            this.RegisterRetry(o2, are1);
            Assert.AreEqual(2, this.triggers.Count);
            Assert.IsTrue(this.triggers.ContainsKey(o2));
            Assert.IsTrue(((Hashtable)this.triggers[o2]).ContainsKey(are1));

            this.RegisterRetry(o1, are2);
            Assert.AreEqual(2, this.triggers.Count);
            Assert.IsTrue(((Hashtable)this.triggers[o1]).ContainsKey(are2));
        }


        [Test]
        public void testRemove()
        {
            object o1 = 1;
            object o2 = 2;

            AutoResetEvent are1 = new AutoResetEvent(false);
            AutoResetEvent are2 = new AutoResetEvent(false);

            this.RegisterRetry(o1, are1);
            this.RegisterRetry(o2, are1);
            this.RegisterRetry(o1, are2);

            this.RemoveRetry(are2);
            Assert.AreEqual(2, this.triggers.Count);
            Assert.IsTrue(((Hashtable)this.triggers[o1]).ContainsKey(are1));
            Assert.IsFalse(((Hashtable)this.triggers[o1]).ContainsKey(are2));
            Assert.IsFalse(((Hashtable)this.triggers[o2]).ContainsKey(are2));

            this.RemoveRetry(are1);
            Assert.AreEqual(2, this.triggers.Count);
            Assert.IsFalse(((Hashtable)this.triggers[o1]).ContainsKey(are1));
            Assert.IsFalse(((Hashtable)this.triggers[o2]).ContainsKey(are1));

            Assert.AreEqual(0, ((Hashtable)this.triggers[o1]).Count);
            Assert.AreEqual(0, ((Hashtable)this.triggers[o2]).Count);
        }


        [Test]
        public void testTake()
        {
            object o1 = 1;
            object o2 = 2;

            AutoResetEvent are1 = new AutoResetEvent(false);
            AutoResetEvent are2 = new AutoResetEvent(false);

            Assert.AreEqual(0, this.TakeAllRetries(o1).Count);

            this.RegisterRetry(o1, are1);
            this.RegisterRetry(o2, are1);
            this.RegisterRetry(o1, are2);

            Assert.AreEqual(2, this.TakeAllRetries(o1).Count);
            Assert.IsFalse(this.triggers.ContainsKey(o1));
            Assert.AreEqual(1, this.TakeAllRetries(o2).Count);
            Assert.IsFalse(this.triggers.ContainsKey(o2));
        }


        [Test]
        public void testNotifyRetriesForTrigger()
        {
            object o1 = 1;
            object o2 = 2;

            AutoResetEvent are1 = new AutoResetEvent(false);
            AutoResetEvent are2 = new AutoResetEvent(false);

            this.RegisterRetry(o1, are1);
            this.RegisterRetry(o2, are1);
            this.RegisterRetry(o1, are2);

            this.NotifyRetriesForTrigger(o1);
            Assert.IsTrue(AutoResetEvent.WaitAll(new WaitHandle[] { are1, are2 }, 0, false));
            Assert.IsFalse(this.triggers.ContainsKey(o1));
        }

        [Test]
        public void testWaitingRetriesSet()
        {
            object o1 = 1;
            object o2 = 2;

            AutoResetEvent are1 = new AutoResetEvent(false);
            AutoResetEvent are2 = new AutoResetEvent(false);
            AutoResetEvent are3 = new AutoResetEvent(false);

            this.RegisterRetry(o1, are1);
            this.RegisterRetry(o2, are1);
            this.RegisterRetry(o1, are2);
            this.RegisterRetry(o2, are3);

            WaitingRetriesSet wrs = new WaitingRetriesSet();
            wrs.Add(this.TakeAllRetries(o2));
            wrs.Add(this.TakeAllRetries(o1));

            Assert.AreEqual(0, this.triggers.Count);

            wrs.NotifyAll();
            Assert.IsTrue(AutoResetEvent.WaitAll(new WaitHandle[] { are1, are2, are3 }, 0, false));
        }
    }
}

#endif