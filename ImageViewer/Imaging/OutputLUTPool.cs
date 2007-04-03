using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal sealed class OutputLUTPool : IReferenceCountable, IDisposable
	{
		private static volatile OutputLUTPool _instance;
		private static object _syncRoot = new Object();

		private ReferenceCountedObjectCache _lutCache;
		private List<OutputLUT> _pool;
		private int _lutPoolSize = 4;
		private int _referenceCount = 0;

		private OutputLUTPool()
		{

		}

		public static OutputLUTPool NewInstance
		{
			get
			{
				if (_instance == null)
				{
					lock (_syncRoot)
					{
						if (_instance == null)
							_instance = new OutputLUTPool();
					}
				}

				_instance.IncrementReferenceCount();

				return _instance;
			}
		}

		private ReferenceCountedObjectCache LutCache
		{
			get
			{
				if (_lutCache == null)
					_lutCache = new ReferenceCountedObjectCache();

				return _lutCache;
			}
		}

		private List<OutputLUT> Pool
		{
			get
			{
				if (_pool == null)
					_pool = new List<OutputLUT>();

				return _pool;
			}
		}

		public OutputLUT Retrieve(string key, int lutSize, out bool composeRequired)
		{
			composeRequired = false;

			// See if we can find the desired LUT in the cache
			OutputLUT lut = this.LutCache[key] as OutputLUT;

			// If not, we'll have to get one from the pool and
			// add it to the cache
			if (lut == null)
			{
				lut = RetrieveFromPool(key, lutSize);
				composeRequired = true;
			}

			this.LutCache.Add(key, lut);
			
			return lut;
		}

		public void Return(string key)
		{
			if (key == String.Empty)
				return;

			OutputLUT lut = this.LutCache[key] as OutputLUT;

			if (lut == null)
				return;

			// If a LUT is being "returned", remove it from the cache first...
			this.LutCache.Remove(key);

			// ...then if no one else is reference the LUT, add it back
			// into the pool so that it can be recycled.
			if (lut.ReferenceCount == 0)
			{
				lut.Key = String.Empty;

				if (this.Pool.Count <= _lutPoolSize)
					this.Pool.Add(lut);
			}
		}

		private OutputLUT RetrieveFromPool(string key, int lutSize)
		{
			// Find a LUT in the pool that's the same size as what's
			// being requested
			foreach (OutputLUT lut in this.Pool)
			{
				// If we've found one, take it out of the pool and return it
				if (lut.LUT.Length == lutSize)
				{
					this.Pool.Remove(lut);
					lut.Key = key;
					return lut;
				}
			}

			// If we couldn't find one, create a new one and return it.  It'll
			// be returned to the pool later when Return is called.
			OutputLUT newLut = new OutputLUT(key, lutSize);
			return newLut;
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
				Platform.Log(e);
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
