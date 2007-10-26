#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class ComposedLutPool : IReferenceCountable, IDisposable
	{
		#region Private Fields

		private static volatile ComposedLutPool _instance;
		private static object _syncRoot = new Object();

		private ReferenceCountedObjectCache _lutCache;
		private List<ComposedLut> _pool;
		private int _lutPoolSize = 4;
		private int _referenceCount = 0;

		#endregion

		private ComposedLutPool()
		{

		}

		#region Public Members

		public static ComposedLutPool NewInstance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncRoot)
					{
						if (_instance == null)
							_instance = new ComposedLutPool();
					}
				}

				_instance.IncrementReferenceCount();

				return _instance;
			}
		}

		public ComposedLut Retrieve(string key, int lutSize, out bool composeRequired)
		{
			composeRequired = false;

			// See if we can find the desired LUT in the cache
			ComposedLut lut = this.LutCache[key] as ComposedLut;

			// If not, we'll have to get one from the pool and
			// add it to the cache
			if (lut == null)
			{
				lut = RetrieveFromPool(lutSize);
				composeRequired = true;
			}

			this.LutCache.Add(key, lut);
			
			return lut;
		}

		public void Return(string key)
		{
			if (key == String.Empty)
				return;

			ComposedLut lut = this.LutCache[key] as ComposedLut;

			if (lut == null)
				return;

			// If a LUT is being "returned", remove it from the cache first...
			this.LutCache.Remove(key);

			// ...then if no one else is reference the LUT, add it back
			// into the pool so that it can be recycled.
			if (lut.ReferenceCount == 0)
			{
				if (this.Pool.Count <= _lutPoolSize)
					this.Pool.Add(lut);
			}
		}

		#region IReferenceCountable Members

		public void IncrementReferenceCount()
		{
			_referenceCount++;
		}

		public void DecrementReferenceCount()
		{
			if (_referenceCount > 0)
				_referenceCount--;
		}

		public bool IsReferenceCountZero
		{
			get { return _referenceCount == 0; }
		}

		public int ReferenceCount
		{
			get { return _referenceCount; }
		}

		#endregion
		#endregion

		#region Private Members

		private ReferenceCountedObjectCache LutCache
		{
			get
			{
				if (_lutCache == null)
					_lutCache = new ReferenceCountedObjectCache();

				return _lutCache;
			}
		}

		private List<ComposedLut> Pool
		{
			get
			{
				if (_pool == null)
					_pool = new List<ComposedLut>();

				return _pool;
			}
		}

		private ComposedLut RetrieveFromPool(int lutSize)
		{
			// Find a LUT in the pool that's the same size as what's
			// being requested
			foreach (ComposedLut lut in this.Pool)
			{
				// If we've found one, take it out of the pool and return it
				if (lut.Data.Length == lutSize)
				{
					this.Pool.Remove(lut);
					return lut;
				}
			}

			// If we couldn't find one, create a new one and return it.  It'll
			// be returned to the pool later when Return is called.
			return new ComposedLut(lutSize);
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
				this.DecrementReferenceCount();

				if (this.IsReferenceCountZero)
				{
					if (_lutCache != null)
					{
						_lutCache.Clear();
						_lutCache = null;
					}

					if (_pool != null)
					{
						_pool.Clear();
						_pool = null;
					}
				}
			}
		}

		#endregion
	}
}
