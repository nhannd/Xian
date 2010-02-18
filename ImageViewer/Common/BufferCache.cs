#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using System.Threading;
using System.Diagnostics;

namespace ClearCanvas.ImageViewer.Common
{
	internal class BufferCache<T>
	{
		private readonly int _maxSize;
		private readonly bool _synchronized;

		private readonly object _syncLock = new object();
		private readonly List<T[]> _buffers = new List<T[]>();

		public BufferCache(int maxSize, bool synchronized)
		{
			_maxSize = maxSize;
			_synchronized = synchronized;
		}

		public T[] Allocate(int length)
		{
			if (_synchronized)
				Monitor.Enter(_syncLock);

			try
			{
				foreach (T[] buffer in _buffers)
				{
					if (buffer.Length == length)
					{
						_buffers.Remove(buffer);
						//Trace.WriteLine("BufferCache: allocated from cache", "Memory");
						return buffer;
					}
				}

				//Trace.WriteLine("BufferCache: allocated from MemoryManager", "Memory");
				return MemoryManager.Allocate<T>(length);
			}
			finally
			{
				if (_synchronized)
					Monitor.Exit(_syncLock);
			}
		}

		public void Return(T[] buffer)
		{
			if (_synchronized)
				Monitor.Enter(_syncLock);

			try
			{
				if (_buffers.Count >= _maxSize)
					_buffers.RemoveAt(0);

				//Trace.WriteLine("BufferCache: buffer returned", "Memory");
				_buffers.Add(buffer);
			}
			finally
			{
				if (_synchronized)
					Monitor.Exit(_syncLock);
			}
		}
	}
}
