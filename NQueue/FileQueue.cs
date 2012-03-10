﻿using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;

namespace ByteNik.Queues
{
    public class FileQueue<T> : IDurableQueue<T>
    {
        public string Path { get; private set; }
        private readonly BinaryFormatter _formatter = new BinaryFormatter();

        public FileQueue(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path); // this should throw if there's a problem

            Path = path;
        }

        public void Enqueue(T item)
        {
            lock (Path)
            {
                var number = Directory.GetFiles(Path).Select(x => int.Parse(System.IO.Path.GetFileName(x))).OrderByDescending(x => x).FirstOrDefault();
                var file = System.IO.Path.Combine(Path, (number + 1).ToString());

                using (var stream = new FileStream(file, FileMode.Create)) _formatter.Serialize(stream, item);

                Monitor.Pulse(Path);
            }
        }

        public void PutBack(T item)
        {
            lock (Path)
            {
                var fname = Directory.GetFiles(Path).Select(System.IO.Path.GetFileName).OrderBy(x => x).FirstOrDefault();
                var number = int.Parse(fname ?? "0"); // get lowest number file
                var file = System.IO.Path.Combine(Path, (number - 1).ToString());

                using (var stream = new FileStream(file, FileMode.Create)) _formatter.Serialize(stream, item);

                Monitor.Pulse(Path);
            }
        }

        public T TryDequeue()
        {
            return TryDequeue(TimeSpan.MaxValue);
        }

        public T TryDequeue(TimeSpan timeout)
        {
            lock (Path)
            {
                var file = Directory.GetFiles(Path).OrderBy(x => int.Parse(System.IO.Path.GetFileName(x))).FirstOrDefault();
                if (file == null)
                {
                    if (timeout == TimeSpan.Zero)
                        throw new IndexOutOfRangeException("No items to dequeue.");
                    
                    // block waiting until timeout or until a file is available
                    bool res;
                    if (timeout == TimeSpan.MaxValue)
                        res = Monitor.Wait(Path);
                    else
                        res = Monitor.Wait(Path, timeout);

                    if(!res) throw new TimeoutException();

                    return TryDequeue(timeout);
                }

                T result;
                using (var stream = new FileStream(file, FileMode.Open)) result = (T) _formatter.Deserialize(stream);
                File.Delete(file);

                return result;
            }
        }

        public void DequeueAsync(Action<T> callback)
        {
            ThreadPool.QueueUserWorkItem(state => callback(TryDequeue()));
        }

        public void Cleanup()
        {
            lock (Path)
            {

            }
        }
    }
}