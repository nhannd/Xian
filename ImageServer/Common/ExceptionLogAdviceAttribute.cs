#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Instructions on how the caller should handle exception thrown by the class
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ExceptionLogAdviceAttribute:Attribute
    {
        /// <summary>
        /// Suppress exception log
        /// </summary>
        public bool Suppress { get; set; }
    }
}
