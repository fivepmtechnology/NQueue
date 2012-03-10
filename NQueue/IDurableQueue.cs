using System;
using System.Threading.Tasks;

namespace ByteNik.Queues
{
    /// <summary>
    /// Represents a durable queue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IDurableQueue<T>
    {
        /// <summary>
        /// Add an item to the queue.
        /// </summary>
        /// <param name="item">the item to enqueue</param>
        void Enqueue(T item);

        /// <summary>
        /// Puts an item back at the front of the queue.
        /// </summary>
        /// <param name="item"></param>
        void PutBack(T item);

        /// <summary>
        /// Dequeues an item from the queue, blocking until one is available.
        /// </summary>
        /// <returns>the dequeued item</returns>
        T Dequeue();
        /// <summary>
        /// Dequeues an item from the queue, blocking until one is available or the timeout elapses.
        /// </summary>
        /// <returns>the dequeued item</returns>
        T Dequeue(TimeSpan timeout);

        /// <summary>
        /// Dequeues an item from the queue, passing it to the callback delegate.
        /// </summary>
        /// <param name="callback"></param>
        Task<T> DequeueAsync();
    }
}
