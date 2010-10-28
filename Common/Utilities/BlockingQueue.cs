#region License (non-CC)

// This software is licensed under the Microsoft Limited Public License,
// the terms of which are listed as follows.
//
// 1. Definitions
//    The terms "reproduce," "reproduction," "derivative works," and
//    "distribution" have the same meaning here as under U.S. copyright law.
//    * A "contribution" is the original software, or any additions or changes to
//      the software.
//    * A "contributor" is any person that distributes its contribution under this
//      license.
//    * "Licensed patents" are a contributor's patent claims that read directly on
//      its contribution.
//
// 2. Grant of Rights
//    (A) Copyright Grant - Subject to the terms of this license, including the
//        license conditions and limitations in section 3, each contributor grants
//        you a non-exclusive, worldwide, royalty-free copyright license to
//        reproduce its contribution, prepare derivative works of its contribution,
//        and distribute its contribution or any derivative works that you create.
//    (B) Patent Grant - Subject to the terms of this license, including the
//        license conditions and limitations in section 3, each contributor grants
//        you a non-exclusive, worldwide, royalty-free license under its licensed
//        patents to make, have made, use, sell, offer for sale, import, and/or
//        otherwise dispose of its contribution in the software or derivative works
//        of the contribution in the software.
//
// 3. Conditions and Limitations
//    (A) No Trademark License - This license does not grant you rights to use any
//        contributors' name, logo, or trademarks.
//    (B) If you bring a patent claim against any contributor over patents that you
//        claim are infringed by the software, your patent license from such
//        contributor to the software ends automatically.
//    (C) If you distribute any portion of the software, you must retain all
//        copyright, patent, trademark, and attribution notices that are present in
//        the software.
//    (D) If you distribute any portion of the software in source code form, you
//        may do so only under this license by including a complete copy of this
//        license with your distribution. If you distribute any portion of the
//        software in compiled or object code form, you may only do so under a
//        license that complies with this license.
//    (E) The software is licensed "as-is." You bear the risk of using it. The
//        contributors give no express warranties, guarantees or conditions. You
//        may have additional consumer rights under your local laws which this
//        license cannot change. To the extent permitted under your local laws,
//        the contributors exclude the implied warranties of merchantability,
//        fitness for a particular purpose and non-infringement.
//    (F) Platform Limitation - The licenses granted in sections 2(A) & 2(B) extend
//        only to the software or derivative works that you create that run on a
//        Microsoft Windows operating system product.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Basic producer-consumer queue, taken from here:
    /// http://blogs.msdn.com/toub/archive/2006/04/12/575103.aspx 
	/// but slightly modified so the thread being blocked can exit and re-enter.
    /// </summary>
    /// <typeparam name="T">The type to be used in the queue.</typeparam>
    public class BlockingQueue<T>
    {
		private object _syncLock = new object();
		private Queue<T> _queue;
		private bool _continueBlocking;

		/// <summary>
		/// Constructor.
		/// </summary>
		public BlockingQueue()
		{
			 _queue = new Queue<T>();
			_continueBlocking = true;
		}

		/// <summary>
		/// Removes the item at the head of the queue.
		/// </summary>
		/// <remarks>
		/// <para>
		/// If no items are available, this call will block until an item becomes available, 
		/// unless the <see cref="ContinueBlocking"/> member has been set to false.
		/// </para>
		/// <para>
		/// This method will not throw an exception.
		/// </para>
		/// </remarks>
		/// <param name="value">The value of the next item in the queue, or <b>default(T)</b> 
		/// if <see cref="ContinueBlocking"/> is false and the queue is empty.</param>
		/// <returns>True if the item returned (via the out parameter) was in the queue, otherwise false.</returns>
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
		/// <exception cref="ArgumentNullException">Thrown when the input item is null.</exception>
		/// <param name="item">The item to enqueue.</param>
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
		/// Indicates whether or not the <b>Dequeue</b> methods should block until the queue
		/// becomes non-empty.
		/// </summary>
		/// <remarks>
		/// When set to false, all actively waiting threads 
		/// (e.g. currently blocked, calling <b>Dequeue</b>) are  released so they 
		/// can determine whether or not they should quit.
		/// </remarks>
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
