using System;
using System.IO;

namespace ClearCanvas.Common
{
    public interface IResourceResolver
    {
        /// <summary>
        /// Attempts to localize the specified unqualified string resource key.
        /// </summary>
        /// <remarks>
        /// Searches for a string resource entry that matches the specified key.
        /// </remarks>
        /// <param name="unqualifiedStringKey">The string resource key to search for.  Must not be qualified.</param>
        /// <returns>The localized string, or the argument unchanged if the key could not be found</returns>
        string LocalizeString(string unqualifiedStringKey);

        /// <summary>
        /// Attempts to return a fully qualified resource name from the specified name, which may be partially
        /// qualified or entirely unqualified.
        /// </summary>
        /// <param name="resourceName">A partially qualified or unqualified resource name</param>
        /// <returns>A qualified resource name, if found, otherwise an exception is thrown</returns>
        /// <exception cref="MissingManifestResourceException">if the resource name could not be resolved</exception>
        Stream OpenResource(string resourceName);


        /// <summary>
        /// Attempts to resolve and open a resource from the specified name, which may be partially
        /// qualified or entirely unqualified.
        /// </summary>
        /// <param name="resourceName">A partially qualified or unqualified resource name</param>
        /// <returns>A qualified resource name, if found, otherwise an exception is thrown</returns>
        /// <exception cref="MissingManifestResourceException">if the resource name could not be resolved</exception>
        string ResolveResource(string resourceName);
    }
}
