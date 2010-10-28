#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common.Utilities
{
	//TODO (CR Sept 2010): move to Tests namespace?
	//TODO (CR Sept 2010): #if UNIT_TESTS?
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
        public virtual object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
        {
            return new object[] { };
        }

        /// <summary>
        /// Returns an empty array.
        /// </summary>
        public virtual ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
        {
            return new ExtensionInfo[] { };
        }

        #endregion
    }
}
