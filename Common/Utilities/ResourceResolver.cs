using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Default implementation of <see cref="IResourceResolver"/>.  Resolves resources by searching the set of
    /// assemblies (specified in the constructor) in order for a matching resource.
    /// </summary>
    public class ResourceResolver : IResourceResolver
    {
        /// <summary>
        /// Cache of string resource managers for each assembly
        /// </summary>
        private static Dictionary<Assembly, List<ResourceManager>> _mapStringResourceManagers = new Dictionary<Assembly, List<ResourceManager>>();

        private Assembly[] _assemblies;

        /// <summary>
        /// Constructs an object that will search the specified set of assemblies.
        /// </summary>
        /// <param name="assemblies">The set of assemblies to search</param>
        public ResourceResolver(Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        /// <summary>
        /// Constructs an object that will search the specified assembly.
        /// </summary>
        /// <param name="assembly">The set of assemblies to search</param>
        public ResourceResolver(Assembly assembly)
            :this(new Assembly[] { assembly })
        {
        }

        /// <summary>
        /// Attempts to localize the specified unqualified string resource key
        /// by searching the set of assemblies associated with this <see cref="ResourceResolver"/> in order.
        /// </summary>
        /// <remarks>
        /// Searches the assemblies for resources ending in "SR.resources", and searches those resources
        /// for a string whose matching the specified key.
        /// </remarks>
        /// <param name="unqualifiedStringKey">The string resource key to search for.  Must not be qualified.</param>
        /// <returns>The localized string, or the argument unchanged if the key could not be found</returns>
        public string LocalizeString(string unqualifiedStringKey)
        {
            // search the assemblies in order
            foreach(Assembly asm in _assemblies)
            {
                try
                {
                    string localized = LocalizeString(unqualifiedStringKey, asm);
                    if (localized != null)
                    {
                        return localized;
                    }
                }
                catch (Exception)
                {
                    // failed to resolve in the specified assembly
                }
            }
            return unqualifiedStringKey;     // return the unresolved string if not resolved
        }

        /// <summary>
        /// Attempts to return a fully qualified resource name from the specified name, which may be partially
        /// qualified or entirely unqualified, by searching the assemblies associated with this <see cref="ResourceResolver"/> in order.
        /// </summary>
        /// <param name="resourceName">A partially qualified or unqualified resource name</param>
        /// <returns>A qualified resource name, if found, otherwise an exception is thrown</returns>
        /// <exception cref="MissingManifestResourceException">if the resource name could not be resolved</exception>
        public string ResolveResource(string resourceName)
        { 
			//resources are qualified internally in the manifest with '.' characters, so let's first try
			//to find a resource that matches 'exactly' by preceding the name with a '.'
			string exactMatch = String.Format(".{0}", resourceName);

			foreach (Assembly asm in _assemblies)
            {
				foreach (string match in GetResourcesEndingWith(asm, exactMatch))
				{
					return match;
				}

				//next we'll just try to find the first match ending with the resource name.
				foreach (string match in GetResourcesEndingWith(asm, resourceName))
                {
                    return match;
                }
            }

			throw new MissingManifestResourceException(SR.ExceptionResourceNotFound);
        }

        /// <summary>
        /// Attempts to resolve and open a resource from the specified name, which may be partially
        /// qualified or entirely unqualified, by searching the assemblies associated with this <see cref="ResourceResolver"/> in order.
        /// </summary>
        /// <param name="resourceName">A partially qualified or unqualified resource name</param>
        /// <returns>A qualified resource name, if found, otherwise an exception is thrown</returns>
        /// <exception cref="MissingManifestResourceException">if the resource name could not be resolved</exception>
        public Stream OpenResource(string resourceName)
        {
			//resources are qualified internally in the manifest with '.' characters, so let's first try
			//to find a resource that matches 'exactly' by preceding the name with a '.'
			string exactMatch = String.Format(".{0}", resourceName);
			
			foreach (Assembly asm in _assemblies)
			{
				foreach (string match in GetResourcesEndingWith(asm, exactMatch))
				{
					return asm.GetManifestResourceStream(match);
				}

				//next we'll just try to find the first match ending with the resource name.
				foreach (string match in GetResourcesEndingWith(asm, resourceName))
				{
					return asm.GetManifestResourceStream(match);
				}
			}

			throw new MissingManifestResourceException(SR.ExceptionResourceNotFound);
        }

        /// <summary>
        /// Attempts to localize the specified string table key from the specified assembly, checking all
        /// string resource file in arbitrary order.  The first match is returned, or null if no matches
        /// are found.
        /// </summary>
        /// <param name="stringTableKey">The string table key to localize</param>
        /// <param name="asm">The assembly to look in</param>
        /// <returns>The first string table entry that matches the specified key, or null if no matches are found</returns>
        private string LocalizeString(string stringTableKey, Assembly asm)
        {
            foreach (ResourceManager resourceManager in GetStringResourceManagers(asm))
            {
                string resolved = resourceManager.GetString(stringTableKey);
                if (resolved != null)
                    return resolved;
            }
            return null;
        }

        /// <summary>
        /// Returns a list of <see cref="ResourceManager"/>, one for each string resource file that is present
        /// in the specified assembly.  The resource manager can then be used to localize strings.
        /// </summary>
        /// <param name="asm"></param>
        /// <returns></returns>
        private static List<ResourceManager> GetStringResourceManagers(Assembly asm)
        {
            if (!_mapStringResourceManagers.ContainsKey(asm))
            {
                List<ResourceManager> resourceManagers = new List<ResourceManager>();
                foreach(string stringResource in GetResourcesEndingWith(asm, "SR.resources"))
                {
                    resourceManagers.Add(new ResourceManager(stringResource.Replace(".resources", ""), asm));
                }
                _mapStringResourceManagers.Add(asm, resourceManagers);
            }
            return _mapStringResourceManagers[asm];
        }

        /// <summary>
        /// Searches the specified assembly for resource files whose names end with the specified string.
        /// </summary>
        /// <param name="asm">The assembly to search</param>
        /// <param name="endingWith">The string to match the end of the resource name with</param>
        /// <returns></returns>
        private static string[] GetResourcesEndingWith(Assembly asm, string endingWith)
        {
            List<string> stringResources = new List<string>();
            foreach (string resName in asm.GetManifestResourceNames())
            {
                if (resName.EndsWith(endingWith))
                    stringResources.Add(resName);
            }
            return stringResources.ToArray();
        }
   }
}
