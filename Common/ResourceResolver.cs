using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.IO;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Utility class that provides runtime string resource resolution services.
    /// </summary>
    /// <remarks>
    /// This class searches a specified set of assemblies for classes named SR,
    /// and searches the properties of the SR classes for a property with a matching name.
    /// </remarks>
    public class ResourceResolver : IResourceResolver
    {
        private static Dictionary<Assembly, List<Type>> _mapAsmToSRclass = new Dictionary<Assembly,List<Type>>();
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
                    string localized = ResolveStringResource(unqualifiedStringKey, asm);
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
            foreach (Assembly asm in _assemblies)
            {
                foreach (string match in GetResourcesEndingWith(asm, resourceName))
                {
                    return match;    // just return the first match
                }
            }

            throw new MissingManifestResourceException("Resource not found") ;
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
            foreach (Assembly asm in _assemblies)
            {
                foreach (string match in GetResourcesEndingWith(asm, resourceName))
                {
                    return asm.GetManifestResourceStream(match);    // just return the first match
                }
            }

            throw new MissingManifestResourceException("Resource not found");
        }

        private string ResolveStringResource(string str, Assembly asm)
        {
            foreach (ResourceManager resourceManager in GetStringResourceManagers(asm))
            {
                string resolved = resourceManager.GetString(str);
                if (resolved != null)
                    return resolved;
            }
            return null;
        }

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
