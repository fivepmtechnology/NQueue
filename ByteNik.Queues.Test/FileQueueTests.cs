using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ByteNik.Queues.Test
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
            var res = _testQueue.TryDequeue();

            Assert.AreEqual(res, value);
        }

        [TestMethod]
        public void EnqueueAndDequeueTwice()
        {
            const string value = "abc";
            _testQueue.Enqueue(value);
            var res = _testQueue.TryDequeue();
            bool fail = true;
            try
            {
                _testQueue.TryDequeue(TimeSpan.FromMilliseconds(10));
            }
            catch
            {
                fail = false;
            }

            Assert.AreEqual(res, value);
            Assert.IsFalse(fail);
        }
    }
}
