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
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Defines the interface to a set of validation rules that may be tested as a single specification.
    /// </summary>
    public interface IValidationRuleSet : ISpecification
    {
        /// <summary>
        /// Tests this rule set against the specified object.  Only rules that are selected by the specified
        /// filter are tested.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        TestResult Test(object obj, Predicate<ISpecification> filter);
    }
}
