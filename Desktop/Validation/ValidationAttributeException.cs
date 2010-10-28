#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
	/// Thrown by <see cref="ValidationAttribute"/> when errors occur creating validation rules.
    /// </summary>
	public class ValidationAttributeException : Exception
    {
        internal ValidationAttributeException(string message)
            :base(message)
        {
        }
    }
}
