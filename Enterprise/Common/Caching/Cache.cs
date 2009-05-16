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
		/// Creates a cache client based on the specified arguments.
		/// </summary>
		/// <remarks>
		/// This method is safe for concurrent use by multiple threads.
		/// </remarks>
		/// <param name="args"></param>
		/// <returns></returns>
		public static ICacheClient CreateClient(CacheClientCreationArgs args)
		{
            // a cacheID is required!
            Platform.CheckForNullReference(args.CacheID, "CacheID");

            // eventually we may allow configuration of logical caches via external config,
            // but for now the expiration time must be provided in the args!
            if (args.ExpirationTime == TimeSpan.Zero)
                throw new ArgumentException("ExpirationTime must be non-zero.");

            // TODO a more sophisticated delegate may be required here
			// if more than one cache provider extension exists, there will need to be mechanisms for choosing
			// the appropriate provider, which may be influenced by a) the creation args,
			// and b) potentially some external configuration settings
			ICacheProvider provider = GetProvider(delegate { return true; });

			// create specified cache
            // this call assumes the provider.CreateClient method is thread-safe, which
            // is the responsibility of the provider!
			return provider.CreateClient(args);
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
			if(extension == null)
				throw new Exception();	//TODO use typed exception

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
