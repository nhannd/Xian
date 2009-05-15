using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Common.Caching
{
	/// <summary>
	/// Defines an extension point for implementations of <see cref="ICacheClientProvider"/>.
	/// </summary>
	[ExtensionPoint]
	public class CacheClientProviderExtensionPoint: ExtensionPoint<ICacheClientProvider>
	{
	}

	/// <summary>
	/// Static factory class for obtaining instances of <see cref="ICacheClient"/>.
	/// </summary>
	public static class CacheClientFactory
	{
		/// <summary>
		/// Maintains the singleton instance of each class of provider.
		/// </summary>
		private static readonly Dictionary<Type, ICacheClientProvider> _providers = new Dictionary<Type, ICacheClientProvider>();


		/// <summary>
		/// Creates a cache client based on the specified arguments.
		/// </summary>
		/// <remarks>
		/// This method is safe for concurrent use by multiple threads.
		/// </remarks>
		/// <param name="args"></param>
		/// <returns></returns>
		public static ICacheClient CreateCacheClient(CacheClientCreationArgs args)
		{
			// TODO a more sophisticated delegate may be required here
			// if more than one cache provider extension exists, there will need to be mechanisms for choosing
			// the appropriate provider, which may be influenced by a) the creation args,
			// and b) potentially some external configuration settings
			ICacheClientProvider provider = GetProvider(delegate { return true; });

			// lock the provider, in case it isn't thread-safe
			lock(provider)
			{
				// create specified cache
				return provider.CreateCacheClient(args);
			}
		}

		private static ICacheClientProvider GetProvider(Predicate<ExtensionInfo> filter)
		{
			// determine the provider class
			CacheClientProviderExtensionPoint point = new CacheClientProviderExtensionPoint();
			ExtensionInfo extension = CollectionUtils.FirstElement(point.ListExtensions(filter));
			if(extension == null)
				throw new Exception();	//TODO use typed exception

			Type providerClass = extension.ExtensionClass;

			// check if we already have an initialized instance of this provider class.
			ICacheClientProvider provider;
			if (!_providers.TryGetValue(providerClass, out provider))
			{
				// if not, create one
				provider = (ICacheClientProvider) point.CreateExtension(
					new ClassNameExtensionFilter(providerClass.FullName));

				lock(_providers)
				{
					// ensure that another thread hasn't beat us to it
					if (!_providers.ContainsKey(providerClass))
					{
						// initialize this provider and store it
						provider.Initialize(new CacheClientProviderInitializationArgs());
						_providers.Add(providerClass, provider);
					}
				}
			}
			return provider;
		}
	}
}
