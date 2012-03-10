using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NQueue.Test
{
    [TestClass]
    public class FileQueueTests
    {
        private FileQueue<string> _testQueue;

        [TestInitialize]
        public void Initialize()
        {
            var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _testQueue = new FileQueue<string>(path);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(_testQueue.Path, true);
        }

        [TestMethod]
        public void SimpleEnqueueAndDequeue()
        {
            const string value = "abc";
            _testQueue.Enqueue(value);
            var res = _testQueue.Dequeue();

            Assert.AreEqual(res, value);
        }

        [TestMethod]
        public void EnqueueAndDequeueTwice()
        {
            const string value = "abc";
            _testQueue.Enqueue(value);
            var res = _testQueue.Dequeue();
            bool fail = true;
            try
            {
                _testQueue.Dequeue(TimeSpan.FromMilliseconds(10));
            }
            catch
            {
                fail = false;
            }

            Assert.AreEqual(res, value);
            Assert.IsFalse(fail);
        }

        [TestMethod]
        public void EquivelentToBuiltInQueueTest()
        {
            var dotNetQueue = new Queue<string>();
            for (int x = 0; x < 25; x++)
            {
                var str = x.ToString();
                dotNetQueue.Enqueue(str);
                _testQueue.Enqueue(str);
            }

            while (dotNetQueue.Count != 0)
                Assert.AreEqual(dotNetQueue.Dequeue(), _testQueue.Dequeue());
        }

        [TestMethod]
        public void ProducerConsumerTest()
        {
            var dotNetQueue = new Queue<string>();
            for (int x = 0; x < 25; x++)
                dotNetQueue.Enqueue(x.ToString());

            ThreadPool.QueueUserWorkItem(state =>
                                                 {
                                                     for (int x = 0; x < 25; x++)
                                                         _testQueue.Enqueue(x.ToString());
                                                 });

            while (dotNetQueue.Count != 0)
                Assert.AreEqual(dotNetQueue.Dequeue(), _testQueue.Dequeue());
        }
    }
}

