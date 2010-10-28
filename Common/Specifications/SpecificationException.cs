#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#pragma warning disable 1591

using System;

namespace ClearCanvas.Common.Specifications
{
    public class SpecificationException : Exception
    {
        public SpecificationException(string message)
            :base(message)
        {

        }

        public SpecificationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
