using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Basic producer-consumer queue, taken from here:
    /// http://blogs.msdn.com/toub/archive/2006/04/12/575103.aspx
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BlockingQueue<T> : IEnumerable<T>
    {
        private int _count = 0;
        private Queue<T> _queue = new Queue<T>();

        /// <summary>
        /// Removes the item at the head of the queue.  If no items are available, this call
        /// will block until an item becomes available.
        /// </summary>
        /// <returns>The item removed from the queue</returns>
        public T Dequeue()
        {
            lock (_queue)
            {
                while (_count <= 0) Monitor.Wait(_queue);
                _count--;
                return _queue.Dequeue();
            }
        }

        /// <summary>
        /// Adds the specified item to the end of the queue.
        /// </summary>
        /// <param name="data">The item to enqueue</param>
        public void Enqueue(T data)
        {
            if (data == null) throw new ArgumentNullException("data");
            lock (_queue)
            {
                _queue.Enqueue(data);
                _count++;
                Monitor.Pulse(_queue);
            }
        }

        /// <summary>
        /// Blocking enumerator.  This enumerator will execute indefinitely, dequeuing items
        /// or blocking when no items are available.
        /// </summary>
        /// <returns>A blocking enumerator that enumerates indefinitely over this queue</returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            while (true) yield return Dequeue();
        }

        /// <summary>
        /// Blocking enumerator.  This enumerator will execute indefinitely, dequeuing items
        /// or blocking when no items are available.
        /// </summary>
        /// <returns>A blocking enumerator that enumerates indefinitely over this queue</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)this).GetEnumerator();
        }
    }
}
