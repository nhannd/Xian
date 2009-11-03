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
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Common
{
	//TODO (cr Oct 2009): Get rid of this class?

	/// <summary>
	/// Static helper class for use when debugging.
	/// </summary>
	public static class Diagnostics
	{
		#region Memory

		private static long _totalLargeObjectMemoryBytes = 0;

		private static readonly object _syncLock = new object(); 
		private static event EventHandler _totalLargeObjectBytesChanged;

		/// <summary>
		/// Gets the running total byte count of large objects held in memory.
		/// </summary>
		public static long TotalLargeObjectBytes
		{
			get
			{
				//synchronize changes, but not reads
				return Thread.VolatileRead(ref _totalLargeObjectMemoryBytes);
			}	
		}

		/// <summary>
		/// Occurs when <see cref="TotalLargeObjectBytes"/> has changed.
		/// </summary>
		public static event EventHandler TotalLargeObjectBytesChanged
		{
			add
			{
				lock(_syncLock)
				{
					_totalLargeObjectBytesChanged += value;
				}
			}
			remove
			{
				lock (_syncLock)
				{
					_totalLargeObjectBytesChanged -= value;
				}
			}
		}

		/// <summary>
		/// Called when a large object is allocated.
		/// </summary>
		/// <remarks>
		/// Although it is not necessary to call this method when you allocate a large object,
		/// such as a byte array for pixel data, it is recommended that you do so in order
		/// for this class to provide accurate data.
		/// </remarks>
		public static void OnLargeObjectAllocated(long bytes)
		{
			lock(_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes += bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Called when a large object is released.
		/// </summary>
		/// <remarks>
		/// You should call this method exactly once when you are certain the large object
		/// in question is no longer in use.
		/// </remarks>
		public static void OnLargeObjectReleased(long bytes)
		{
			lock (_syncLock)
			{
				//synchronize changes, but not reads
				_totalLargeObjectMemoryBytes -= bytes;
				EventsHelper.Fire(_totalLargeObjectBytesChanged, null, EventArgs.Empty);
			}
		}

		#endregion
	}
}
