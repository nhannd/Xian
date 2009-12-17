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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	/// <summary>
	/// <see cref="EventArgs"/> for <see cref="IMemoryManagementStrategy.MemoryCollected"/> events.
	/// </summary>
	public class MemoryCollectedEventArgs : EventArgs
	{
		private bool _needMoreMemory = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		public MemoryCollectedEventArgs(int largeObjectContainersUnloadedCount,
			int largeObjectsCollectedCount, long bytesCollectedCount, TimeSpan elapsedTime, bool isLast)
		{
			ElapsedTime = elapsedTime;
			LargeObjectContainersUnloadedCount = largeObjectContainersUnloadedCount;
			LargeObjectsCollectedCount = largeObjectsCollectedCount;
			BytesCollectedCount = bytesCollectedCount;
			IsLast = isLast;
		}

		/// <summary>
		/// The total time taken to collect memory.
		/// </summary>
		public readonly TimeSpan ElapsedTime;

		/// <summary>
		/// The total number of <see cref="ILargeObjectContainer"/>s that were unloaded.
		/// </summary>
		public readonly int LargeObjectContainersUnloadedCount;
		/// <summary>
		/// The total number of large objects collected.
		/// </summary>
		public readonly int LargeObjectsCollectedCount;
		/// <summary>
		/// The total number of bytes collected.
		/// </summary>
		public readonly long BytesCollectedCount;
		/// <summary>
		/// Indicates whether or not this is the last <see cref="IMemoryManagementStrategy.MemoryCollected"/> event
		/// for the current collection.
		/// </summary>
		public readonly bool IsLast;

		/// <summary>
		/// Gets or sets whether or not more memory is needed by any entity currently observing the
		/// <see cref="IMemoryManagementStrategy.MemoryCollected"/> event.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The <see cref="IMemoryManagementStrategy">memory management strategy</see> will look at this value
		/// when determining whether or not to continue collecting memory.
		/// </para>
		/// <para>
		/// Once set to true, the value cannot be set back to false.
		/// </para>
		/// </remarks>
		public bool NeedMoreMemory
		{
			get { return _needMoreMemory; }
			set
			{
				if (value)
					_needMoreMemory = true;
			}
		}
	}

	/// <summary>
	/// Argument class passed to the <see cref="IMemoryManagementStrategy"/>.
	/// </summary>
	public class MemoryCollectionArgs
	{
		internal MemoryCollectionArgs(IEnumerable<ILargeObjectContainer> largeObjectContainers)
		{
			LargeObjectContainers = largeObjectContainers;
		}

		/// <summary>
		/// Gets all the <see cref="ILargeObjectContainer"/>s currently being managed by the <see cref="MemoryManager"/>.
		/// </summary>
		public readonly IEnumerable<ILargeObjectContainer> LargeObjectContainers;
	}

	/// <summary>
	/// Defines the interface to a memory management strategy.
	/// </summary>
	/// <remarks>
	/// Implementers must mark their class as an extension of <see cref="MemoryManagementStrategyExtensionPoint"/> in order
	/// to override the default strategy.
	/// </remarks>
	public interface IMemoryManagementStrategy
	{
		/// <summary>
		/// Called by the <see cref="MemoryManager"/> to collect memory from
		/// <see cref="ILargeObjectContainer"/>s, if necessary.  See <see cref="MemoryManager"/> for more details.
		/// </summary>
		void Collect(MemoryCollectionArgs collectionArgs);
		/// <summary>
		/// Fired when memory has been collected.
		/// </summary>
		/// <remarks>The event must be fired at least once, but may be fired repeatedly in order to try
		/// and return control to waiting threads as quickly as possible.
		/// </remarks>
		event EventHandler<MemoryCollectedEventArgs> MemoryCollected;
	}

	/// <summary>
	/// Abstract base implementation <see cref="IMemoryManagementStrategy"/>.
	/// </summary>
	public abstract class MemoryManagementStrategy : IMemoryManagementStrategy
	{
		private class NullMemoryManagementStrategy : IMemoryManagementStrategy
		{
			#region IMemoryManagementStrategy Members

			public void Collect(MemoryCollectionArgs collectionArgs)
			{
			}

			public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
			{
				add { }
				remove { }
			}

			#endregion
		}

		internal static readonly IMemoryManagementStrategy Null = new NullMemoryManagementStrategy();

		private event EventHandler<MemoryCollectedEventArgs> _memoryCollected;
		
		/// <summary>
		/// Protected constructor.
		/// </summary>
		protected MemoryManagementStrategy()
		{
		}

		#region IMemoryManagementStrategy Members

		/// <summary>
		/// Called by the <see cref="MemoryManager"/> to collect memory from
		/// <see cref="ILargeObjectContainer"/>s, if necessary.  See <see cref="MemoryManager"/> for more details.
		/// </summary>
		public abstract void Collect(MemoryCollectionArgs collectionArgs);

		/// <summary>
		/// Fired when memory has been collected.
		/// </summary>
		/// <remarks>The event must be fired at least once, but may be fired repeatedly in order to try
		/// and return control to waiting threads as quickly as possible.
		/// </remarks>
		public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add { _memoryCollected += value; }
			remove { _memoryCollected -= value; }
		}

		#endregion

		/// <summary>
		/// Fires the <see cref="MemoryCollected"/> event.
		/// </summary>
		protected void OnMemoryCollected(MemoryCollectedEventArgs args)
		{
			try
			{
				EventsHelper.Fire(_memoryCollected, this, args);
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unexpected failure while firing memory collected event.");
			}
		}
	}
}
