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

namespace ClearCanvas.ImageViewer.Imaging
{
	internal delegate void ComposeLutDelegate(int[] lut);

	internal sealed class ComposedLutPool : IDisposable
	{
		#region Private Fields

		private static readonly object _syncLock = new object();
		private static readonly ComposedLutPool _instance = new ComposedLutPool();

		private int _referenceCount;
		private readonly Dictionary<string, ReferenceCountedObjectWrapper<int[]>> _lutCache = 
			new Dictionary<string, ReferenceCountedObjectWrapper<int[]>>();
		
		private readonly List<int[]> _pool = new List<int[]>();
		private readonly int _lutPoolSize = 4;

		#endregion

		private ComposedLutPool()
		{
			_referenceCount = 0;
		}

		#region Public Members

		public static ComposedLutPool NewInstance
		{
			get
			{
				lock (_syncLock)
				{
					++_instance._referenceCount;
				}

				return _instance;
			}
		}

		public int[] Retrieve(string key, int lutSize, ComposeLutDelegate composeLutDelegate)
		{
			Platform.CheckForNullReference(composeLutDelegate, "composeLutDelegate");

			lock (_syncLock)
			{
				if (!LutCache.ContainsKey(key))
				{
					LutCache[key] = new ReferenceCountedObjectWrapper<int[]>(RetrieveFromPool(lutSize));

					//Compose right away, so the operation is synchronized.
					composeLutDelegate(LutCache[key].Item);
				}

				LutCache[key].IncrementReferenceCount();
				return LutCache[key].Item;
			}
		}
	
		public void Return(string key)
		{
			if (String.IsNullOrEmpty(key))
				return;

			lock (_syncLock)
			{
				if (!LutCache.ContainsKey(key))
					return;

				ReferenceCountedObjectWrapper<int[]> wrapper = LutCache[key];
				wrapper.DecrementReferenceCount();
				if (!wrapper.IsReferenceCountAboveZero())
				{
					LutCache.Remove(key);

					if (this.Pool.Count <= _lutPoolSize)
						this.Pool.Add(wrapper.Item);
				}
			}
		}

		#endregion

		#region Private Members

		private Dictionary<string, ReferenceCountedObjectWrapper<int[]>> LutCache
		{
			get { return _lutCache; }
		}

		private List<int[]> Pool
		{
			get { return _pool; }
		}

		private int[] RetrieveFromPool(int lutSize)
		{
			// no lock since the calling method itself is synchronized.

			// Find a LUT in the pool that's the same size as what's
			// being requested
			foreach (int[] lut in this.Pool)
			{
				// If we've found one, take it out of the pool and return it
				if (lut.Length == lutSize)
				{
					this.Pool.Remove(lut);
					return lut;
				}
			}

			// If we couldn't find one, create a new one and return it.  It'll
			// be returned to the pool later when Return is called.
			return new int[lutSize];
		}

		#endregion

		#region Disposal

		#region IDisposable Members

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				// shouldn't throw anything from inside Dispose()
				Platform.Log(LogLevel.Error, e);
			}
		}

		#endregion

		/// <summary>
		/// Implementation of the <see cref="IDisposable"/> pattern
		/// </summary>
		/// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				lock (_syncLock)
				{
					if (_referenceCount > 0)
						--_referenceCount;

					if (_referenceCount <= 0)
					{
						_lutCache.Clear();
						_pool.Clear();
					}
				}
			}
		}

		#endregion
	}
}
