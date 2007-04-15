using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Basic producer-consumer queue, taken from here:
    /// http://blogs.msdn.com/toub/archive/2006/04/12/575103.aspx 
	/// but slightly modified so the thread being blocked can exit and re-enter.
    /// </summary>
    /// <typeparam name="T">the type to be used in the queue</typeparam>
    public class BlockingQueue<T>
    {
		private object _syncLock = new object();
		private Queue<T> _queue;
		private bool _continueBlocking;

		public BlockingQueue()
		{
			 _queue = new Queue<T>();
			_continueBlocking = true;
		}

        /// <summary>
        /// Removes the item at the head of the queue.  If no items are available, this call
		/// will block until an item becomes available, unless the <see cref="ContinueBlocking"/> member
		/// has been set to false.
        /// </summary>
		/// <remarks>Note that if you use this method, you must be prepared to catch the exception on whatever thread(s) are currently blocked
		/// in a call to Dequeue() because you will ultimately have to release the threads by setting the <see cref="ContinueBlocking"/> member to false.
		/// That being said, it is preferable to use the alternate <see cref="Dequeue(out T)"/> method unless there is a compelling reason to
		/// use this one.</remarks>
		/// <exception cref="InvalidOperationException">if the queue is empty and the <see cref="ContinueBlocking"/> member is false.</exception> 
        /// <returns>The item removed from the queue.</returns>
        public T Dequeue()
        {
			lock (_syncLock)
            {
				while (_continueBlocking && _queue.Count == 0)
					Monitor.Wait(_syncLock);

				//this will throw an InvalidOperationException if the queue is empty.
                return _queue.Dequeue();
            }
        }

		/// <summary>
		/// Removes the item at the head of the queue.  If no items are available, this call
		/// will block until an item becomes available, unless the <see cref="ContinueBlocking"/> member
		/// has been set to false.
		/// </summary>
		/// <remarks>This method will not throw an exception.</remarks>
		/// <param name="value">the value of the next item in the queue, or default(T) if <see cref="ContinueBlocking"/> is false and the queue is empty.</param>
		/// <returns>true if the item returned (via the out parameter) was in the queue, otherwise false.</returns>
		public bool Dequeue(out T value)
		{
			value = default(T);

			lock (_syncLock)
			{
				while (_continueBlocking && _queue.Count == 0)
					Monitor.Wait(_syncLock);

				if (_queue.Count == 0)
					return false;

				value = _queue.Dequeue();
			}

			return true;
		}
		
		/// <summary>
        /// Adds the specified item to the end of the queue.
        /// </summary>
		/// <exception cref="ArgumentNullException">thrown when the input item is null</exception>
		/// <param name="item">The item to enqueue</param>
        public void Enqueue(T item)
        {
			Platform.CheckForNullReference(item, "item");
            lock (_syncLock)
            {
				_queue.Enqueue(item);
				Monitor.Pulse(_syncLock);
            }
        }

		/// <summary>
		/// Indicates whether or not <see cref="Dequeue"/> should block until the queue
		/// becomes non-empty.  When set to false, all actively waiting threads 
		/// (e.g. currently blocked, calling <see cref="Dequeue"/>) are  released so they 
		/// can determine whether or not they should quit.
		/// </summary>
		public bool ContinueBlocking
		{
			get 
			{
				lock (_syncLock)
				{
					return _continueBlocking;
				}
			}
			set
			{
				lock (_syncLock)
				{
					_continueBlocking = value;
					if (!_continueBlocking)
					{
						//release all waiting threads.
						Monitor.PulseAll(_syncLock);
					}
				}
			}
		}

		/// <summary>
		/// Returns the number of items remaining in the queue.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_syncLock)
				{
					return _queue.Count;
				}
			}
		}
	}
}
