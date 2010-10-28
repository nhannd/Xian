#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Identifies a rule as being logically bound to one or more properties of an object. In other words,
    /// the rule is strictly a function of the values of the specified properties, and is not a function of
    /// anything else.
    /// </summary>
    public interface IPropertyBoundRule
    {
        /// <summary>
        /// Gets an array of the properties of which this rule is a function.
        /// </summary>
        PropertyInfo[] Properties { get; }
    }
}
