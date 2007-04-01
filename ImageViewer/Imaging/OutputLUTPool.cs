using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Imaging
{
	internal class OutputLUTPool
	{
		private ReferenceCountedObjectCache _lutCache = new ReferenceCountedObjectCache();
		private List<OutputLUT> _pool = new List<OutputLUT>();
		private int _lutPoolSize = 4;

		public OutputLUTPool()
		{

		}

		public OutputLUT Retrieve(string key, int lutSize)
		{
			// See if we can find the desired LUT in the cache
			OutputLUT lut = _lutCache[key] as OutputLUT;

			// If not, we'll have to get one from the pool and
			// add it to the cache
			if (lut == null)
			{
				lut = RetrieveFromPool(key, lutSize);
				_lutCache.Add(key, lut);
			}

			return lut;
		}

		public void Return(OutputLUT lut)
		{
			if (lut == null)
				return;

			// If a LUT is being "returned", remove it from the cache first...
			_lutCache.Remove(lut.Key);
			
			// ...then if no one else is reference the LUT, add it back
			// into the pool so that it can be recycled.
			if (lut.ReferenceCount == 0)
			{
				lut.Key = String.Empty;

				if (_pool.Count <= _lutPoolSize)
					_pool.Add(lut);
			}
		}

		private OutputLUT RetrieveFromPool(string key, int lutSize)
		{
			// Find a LUT in the pool that's the same size as what's
			// being requested
			foreach (OutputLUT lut in _pool)
			{
				// If we've found one, take it out of the pool and return it
				if (lut.LUT.Length == lutSize)
				{
					_pool.Remove(lut);
					lut.Key = key;
					return lut;
				}
			}

			// If we couldn't find one, create a new one and return it.  It'll
			// be returned to the pool later when Return is called.
			OutputLUT newLut = new OutputLUT(key, lutSize);
			return newLut;
		}
	}
}
