using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Defines an interface to allow a class to define its own custom cache key string.
	/// </summary>
    public interface IDefinesCacheKey
    {
		/// <summary>
		/// Gets the cache key defined by this instance.
		/// </summary>
		/// <returns></returns>
        string GetCacheKey();
    }
}
