#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;

namespace ClearCanvas.ImageServer.Services.ServiceLock
{
		/// <summary>
	/// Delegate used with <see cref="ServiceLockThreadPool"/> class.
	/// </summary>
	/// <param name="processor">The ServiceLock processor.</param>
	/// <param name="queueItem">The actual ServiceLock item.</param>
	public delegate void ServiceLockThreadDelegate(IServiceLockItemProcessor processor, Model.ServiceLock queueItem);

	/// <summary>
	/// Class used to pass parameters to threads in the <see cref="ServiceLockThreadPool"/>.
	/// </summary>
	public class ServiceLockThreadParameter
	{
		private readonly IServiceLockItemProcessor _processor;
		private readonly Model.ServiceLock _item;
		private readonly ServiceLockThreadDelegate _del;

		public ServiceLockThreadParameter(IServiceLockItemProcessor processor, Model.ServiceLock item, ServiceLockThreadDelegate del)
		{
			_item = item;
			_processor = processor;
			_del = del;
		}

		public IServiceLockItemProcessor Processor
		{
			get { return _processor; }
		}

		public Model.ServiceLock Item
		{
			get { return _item; }
		}

		public ServiceLockThreadDelegate Delegate
		{
			get { return _del; }
		}
	}

	/// <summary>
	/// Thread pool for handling ServiceLock entries, which cancels in progress entries.
	/// </summary>
	public class ServiceLockThreadPool : ItemProcessingThreadPool<ServiceLockThreadParameter>
	{
		#region Private Members
		private readonly object _syncLock = new object();
		private readonly List<ServiceLockThreadParameter> _queuedItems;
		#endregion

		#region Properties
		/// <summary>
		/// Are there threads available for queueing?
		/// </summary>
		public bool CanQueueItem
		{
			get
			{
				return (QueueCount + ActiveCount) < Concurrency;
			}
		}
		#endregion

		#region Contructors
		/// <summary>
		/// Constructors.
		/// </summary>
		/// <param name="totalThreadCount">Total threads to be put in the thread pool.</param>
		public ServiceLockThreadPool(int totalThreadCount)
			: base(totalThreadCount)
		{
			_queuedItems = new List<ServiceLockThreadParameter>(totalThreadCount + 1);
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Override of OnStop method.
		/// </summary>
		/// <param name="completeBeforeStop"></param>
		/// <returns></returns>
		protected override bool OnStop(bool completeBeforeStop)
		{
			if (!base.OnStop(completeBeforeStop))
				return false;
			lock (_syncLock)
			{
				foreach (ServiceLockThreadParameter queuedItem in _queuedItems)
				{
					ICancelable cancel = queuedItem.Processor as ICancelable;
					if (cancel != null)
						cancel.Cancel();
				}
			}
			return true;
		}
		#endregion


		/// <summary>
		/// Method called when a <see cref="ServiceLock"/> item completes.
		/// </summary>
		/// <param name="queueItem">The queue item completing.</param>
		private void QueueItemComplete(Model.ServiceLock queueItem)
		{
			lock (_syncLock)
			{
				foreach (ServiceLockThreadParameter queuedItem in _queuedItems)
				{
					if (queuedItem.Item.Key.Equals(queueItem.Key))
					{
						_queuedItems.Remove(queuedItem);
						break;
					}
				}
			}
		}

		public void Enqueue(IServiceLockItemProcessor processor, Model.ServiceLock item, ServiceLockThreadDelegate del)
		{
			ServiceLockThreadParameter parameter = new ServiceLockThreadParameter(processor, item, del);

			lock (_syncLock)
			{
				_queuedItems.Add(parameter);
			}

			Enqueue(parameter, delegate(ServiceLockThreadParameter threadParameter)
									{
										threadParameter.Delegate(threadParameter.Processor, threadParameter.Item);

										QueueItemComplete(threadParameter.Item);
									});
		}
	}
}
