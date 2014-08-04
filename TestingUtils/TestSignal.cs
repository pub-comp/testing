using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PubComp.Testing.TestingUtils
{
    public class TestSignal
    {
        private static ConcurrentDictionary<int, TestSignal> _instances = new ConcurrentDictionary<int, TestSignal>();

        private readonly Semaphore startSignal = new Semaphore(1, 200);
        private readonly Semaphore completeSignal = new Semaphore(0, 200);
        private readonly int id;
        private long sleepTime;

        public static TestSignal[] Instances
        {
            get
            {
                return _instances.ToArray().Select(i => i.Value).ToArray();
            }
        }

        public static TestSignal Instance
        {
            get
            {
                var key = Thread.CurrentThread.ManagedThreadId;
                var instance = _instances.GetOrAdd(key, k => new TestSignal());
                return instance;
            }
        }

        public static void ClearInstances()
        {
            _instances.Clear();
        }

        protected TestSignal()
        {
            id = Thread.CurrentThread.ManagedThreadId;
        }

        public int Id
        {
            get
            {
                return this.id;
            }
        }

        public void GiveStartSignal()
        {
            startSignal.Release(1);
        }

        public void WaitForStartSignal()
        {
            var sleep = (int)Interlocked.Read(ref sleepTime);
            if (sleep > 0)
                Thread.Sleep(sleep);
            startSignal.WaitOne();
        }

        public void AddSleepBeforeStarting(int sleep)
        {
            Interlocked.Add(ref sleepTime, sleep);
        }

        public void GiveAllStartSignals()
        {
            startSignal.Release(100);
        }

        public void GiveCompleteSignal()
        {
            completeSignal.Release(1);
        }

        public void WaitForCompleteSignal()
        {
            completeSignal.WaitOne();
        }

        public static void WaitForInstances(int count)
        {
            while (_instances.Count < count) { }
            if (_instances.Count > count)
                Assert.Fail("Too many TestSignal instances");
        }
    }
}
