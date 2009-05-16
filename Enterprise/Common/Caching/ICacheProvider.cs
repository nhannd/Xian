using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common.Caching
{
    /// <summary>
    /// Defines an interfaces to an object that provides a cache implementation.
    /// </summary>
	public interface ICacheProvider
	{
		/// <summary>
		/// Initializes this cache provider.
		/// </summary>
		void Initialize(CacheProviderInitializationArgs args);


		/// <summary>
		/// Creates a cache client using the specified arguments.
		/// </summary>
        /// <remarks>
        /// The implementation of this method *must* be safe for multiple threads making concurrent calls.
        /// </remarks>
		/// <returns></returns>
		ICacheClient CreateClient(CacheClientCreationArgs args);
	}
}
