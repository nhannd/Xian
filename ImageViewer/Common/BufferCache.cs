#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Threading;

namespace ClearCanvas.ImageViewer.Common
{
	public class BufferCache<T>
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
