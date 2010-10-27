#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.ImageServer.Common.Exceptions
{
    /// <summary>
    /// Represents the exception thrown when a SOP Instance is processed.
    /// </summary>
    public class SopInstanceProcessingException : Exception
    {
        public SopInstanceProcessingException(string desc):base(desc){}
    }
}