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
	public class MemoryCollectedEventArgs : EventArgs
	{
		private bool _needMoreMemory = false;

		public MemoryCollectedEventArgs(int largeObjectContainersUnloadedCount,
			int largeObjectsCollectedCount, long bytesCollectedCount, TimeSpan elapsedTime, bool isLast)
		{
			ElapsedTime = elapsedTime;
			LargeObjectContainersUnloadedCount = largeObjectContainersUnloadedCount;
			LargeObjectsCollectedCount = largeObjectsCollectedCount;
			BytesCollectedCount = bytesCollectedCount;
			IsLast = isLast;
		}

		public readonly TimeSpan ElapsedTime;
		public readonly int LargeObjectContainersUnloadedCount;
		public readonly int LargeObjectsCollectedCount;
		public readonly long BytesCollectedCount;
		public readonly bool IsLast;

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

	public class MemoryCollectionArgs
	{
		internal MemoryCollectionArgs(IEnumerable<ILargeObjectContainer> largeObjectContainers)
		{
			LargeObjectContainers = largeObjectContainers;
		}

		public readonly IEnumerable<ILargeObjectContainer> LargeObjectContainers;
	}

	public interface IMemoryManagementStrategy
	{
		void Collect(MemoryCollectionArgs collectionArgs);
		event EventHandler<MemoryCollectedEventArgs> MemoryCollected;
	}

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
		
		protected MemoryManagementStrategy()
		{
		}

		#region IMemoryManagementStrategy Members

		public abstract void Collect(MemoryCollectionArgs collectionArgs);

		public event EventHandler<MemoryCollectedEventArgs> MemoryCollected
		{
			add { _memoryCollected += value; }
			remove { _memoryCollected -= value; }
		}

		#endregion

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
