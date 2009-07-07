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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Defines an extension point for implementations of <see cref="ICacheProvider"/>.
	/// </summary>
	[ExtensionPoint]
	public class CacheProviderExtensionPoint: ExtensionPoint<ICacheProvider>
	{
	}

	/// <summary>
	/// Static class providing access to the global singleton appliction cache.
	/// </summary>
	public static class Cache
	{
		/// <summary>
		/// Maintains the singleton instance of each class of provider.
		/// </summary>
		private static readonly Dictionary<Type, ICacheProvider> _providers = new Dictionary<Type, ICacheProvider>();

        /// <summary>
        /// Gets a value indicating if the cache is supported in this environment.
        /// </summary>
        /// <returns></returns>
        public static bool IsSupported()
        {
            CacheProviderExtensionPoint point = new CacheProviderExtensionPoint();
            return point.ListExtensions().Length > 0;
        }

		/// <summary>
		/// Creates a cache client for the specified logical cacheID.
		/// </summary>
		/// <remarks>
		/// This method is safe for concurrent use by multiple threads.
		/// </remarks>
		public static ICacheClient CreateClient(string cacheID)
		{
            // a cacheID is required!
            Platform.CheckForEmptyString(cacheID, "CacheID");

            // TODO a more sophisticated delegate may be required here
			// if more than one cache provider extension exists, there will need to be mechanisms for choosing
			// the appropriate provider, which may be influenced by a) the creation args,
			// and b) potentially some external configuration settings
			ICacheProvider provider = GetProvider(delegate { return true; });

			// create specified cache
            // this call assumes the provider.CreateClient method is thread-safe, which
            // is the responsibility of the provider!
            ICacheClient client = provider.CreateClient(cacheID);

			// if debug logging enabled, wrap client in a logging decorator set at Debug level
			if (Platform.IsLogLevelEnabled(LogLevel.Debug))
			{
				client = new CacheClientLoggingDecorator(client, LogLevel.Debug);
			}
			return client;
		}

        /// <summary>
        /// Thread-safe method to obtain singleton instance of <see cref="ICacheProvider"/>
        /// matching specified filter.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
		private static ICacheProvider GetProvider(Predicate<ExtensionInfo> filter)
		{
			// determine the provider class
			CacheProviderExtensionPoint point = new CacheProviderExtensionPoint();
			ExtensionInfo extension = CollectionUtils.FirstElement(point.ListExtensions(filter));
			if (extension == null)
				throw new CacheException("No cache provider extension found, or those that exist do not support all required features.");

			Type providerClass = extension.ExtensionClass;

			// check if we already have an initialized instance of this provider class.
			ICacheProvider provider;
			if (!_providers.TryGetValue(providerClass, out provider))
			{
				// if not, create one
				provider = (ICacheProvider) point.CreateExtension(
					new ClassNameExtensionFilter(providerClass.FullName));

				lock(_providers)
				{
					// ensure that another thread hasn't beat us to it
					if (!_providers.ContainsKey(providerClass))
					{
						// initialize this provider and store it
						provider.Initialize(new CacheProviderInitializationArgs());
						_providers.Add(providerClass, provider);
					}
				}
			}
			return provider;
		}
	}
}
