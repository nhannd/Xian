using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Defines an interface to allow an object to provide its own custom cache key string.
	/// </summary>
    public interface ICacheKeyProvider
    {
        string GetCacheKey();
    }
}
