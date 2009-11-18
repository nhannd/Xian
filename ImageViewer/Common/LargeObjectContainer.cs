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
using System.Threading;

namespace ClearCanvas.ImageViewer.Common
{
	public enum RegenerationCost
	{
		Low = 0,
		Medium = 1,
		High = 2
	}

	public class LargeObjectContainerData : ILargeObjectContainer
	{
		private readonly Guid _identifier;
		private int _lockCount;
		private volatile int _largeObjectCount;
		private long _totalBytesHeld;
		private volatile RegenerationCost _regenerationCost;
		private DateTime _lastAccessTime;
		private uint _lastAccessTimeAccuracyMilliseconds = 500;
		private int _lastAccessUpdateTickCount;

		public LargeObjectContainerData(Guid identifier)
		{
			_identifier = identifier;
		}

		#region ILargeObjectContainer Members

		public Guid Identifier
		{
			get { return _identifier; }
		}

		public int LargeObjectCount
		{
			get { return _largeObjectCount; }
			set { _largeObjectCount = value; }
		}

		public long BytesHeldCount
		{
			get { return _totalBytesHeld; }
			set { _totalBytesHeld = value; }
		}

		public uint LastAccessTimeAccuracyMilliseconds
		{
			get { return _lastAccessTimeAccuracyMilliseconds; }
			set { _lastAccessTimeAccuracyMilliseconds = value; }
		}

		public DateTime LastAccessTime
		{
			get { return _lastAccessTime; }
		}

		public RegenerationCost RegenerationCost
		{
			get { return _regenerationCost; }
			set { _regenerationCost = value; }
		}

		public bool IsLocked
		{
			get { return Thread.VolatileRead(ref _lockCount) > 0; }
		}

		public void UpdateLastAccessTime()
		{
			//DateTime.Now is extremely expensive if called in a tight loop, so we minimize the potential impact
			//of this problem occurring by only updating the last access time every second or so.
			if (Environment.TickCount - _lastAccessUpdateTickCount < _lastAccessTimeAccuracyMilliseconds)
				return;

			_lastAccessUpdateTickCount = Environment.TickCount;
			_lastAccessTime = DateTime.Now;
		}

		public void Lock()
		{
			Interlocked.Increment(ref _lockCount);
		}

		public void Unlock()
		{
			Interlocked.Decrement(ref _lockCount);
		}

		public void Unload()
		{
			throw new Exception("The method or operation is not implemented.");
		}

		#endregion
	}

	public interface ILargeObjectContainer
	{
		Guid Identifier { get; }

		int LargeObjectCount { get; }
		long BytesHeldCount { get; }

		DateTime LastAccessTime { get; }
		RegenerationCost RegenerationCost { get; }
		bool IsLocked { get; }

		void Lock();
		void Unlock();

		void Unload();
	}
}
