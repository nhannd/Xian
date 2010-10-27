#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Used internally by the framework to indicate an error when attempting to load/use a service.
    /// </summary>
	public class UnknownServiceException : Exception
    {
        internal UnknownServiceException(string message)
            :base(message)
        {
        }
    }
}
