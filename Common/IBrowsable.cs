#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common
{
    /// <summary>
    /// Used by framework to provide a consistent interface for browsable meta-data objects.
    /// </summary>
    public interface IBrowsable
    {
        /// <summary>
        /// Formal name of this object, typically the type name or assembly name.  Cannot be null.
        /// </summary>
        string FormalName
        {
            get;
        }

        /// <summary>
        /// Friendly name of the object, if one exists, otherwise null.
        /// </summary>
        string Name
        {
            get;
        }

        /// <summary>
        /// A friendly description of this object, if one exists, otherwise null.
        /// </summary>
        string Description
        {
            get;
        }
    }
}
