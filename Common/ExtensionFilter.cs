using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common
{
    /// <summary>
    /// An abstract base class for extension filters.  Extension filters are used to filter the
    /// extension points returned by <see cref="Platform.CreateExtensions"/>.  Subclasses of this
    /// class implement specific types of filters.
    /// </summary>
    public abstract class ExtensionFilter
    {
        /// <summary>
        /// Tests the specified extension against the criteria of this filter.
        /// </summary>
        /// <param name="extension">The extension to test</param>
        /// <returns>true if the extension meets the criteria, false otherwise</returns>
        public abstract bool Test(ExtensionInfo extension);
    }
}
