using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// An implementation of <see cref="IExtensionFactory"/> that returns no extensions.
    /// </summary>
    /// <remarks>
    /// This implementation simply returns zero extensions for any extension point.  This is useful
    /// for unit-testing scenarios to prevent any extensions from being inadvertantly created.  This class
    /// may also be used as a base class for a more specialized extension factory that may respond to requests
    /// for certain extension points but not for others.
    /// </remarks>
    public class NullExtensionFactory : IExtensionFactory
    {
        #region IExtensionFactory Members

        /// <summary>
        /// Return an empty array.
        /// </summary>
        /// <param name="extensionPoint"></param>
        /// <param name="filter"></param>
        /// <param name="justOne"></param>
        /// <returns></returns>
        public virtual object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            return new object[] { };
        }

        /// <summary>
        /// Returns an empty array.
        /// </summary>
        /// <param name="extensionPoint"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public virtual ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
            return new ExtensionInfo[] { };
        }

        #endregion
    }
}
