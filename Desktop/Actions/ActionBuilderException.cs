#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop.Actions
{
    ///<summary>
    ///Exception that indicates a problem with the way action attributes are applied to a tool.
    ///</summary>
    public class ActionBuilderException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        internal ActionBuilderException(string message)
            : base(message)
        {
        }
    }
}
