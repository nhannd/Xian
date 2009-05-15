using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
	public interface ICacheClientProvider
	{
		/// <summary>
		/// Initializes this cache provider.
		/// </summary>
		void Initialize(CacheClientProviderInitializationArgs args);


		/// <summary>
		/// Creates a cache client using the specified arguments.
		/// </summary>
		/// <returns></returns>
		ICacheClient CreateCacheClient(CacheClientCreationArgs args);
	}
}
