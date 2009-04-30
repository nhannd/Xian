#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common;
using System.Collections.ObjectModel;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Encapsulates a set of 
    /// Note: immutable
    /// </summary>
    public class ValidationRuleSet : IValidationRuleSet
    {
        private List<ISpecification> _rules;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationRuleSet()
        {
            _rules = new List<ISpecification>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rules"></param>
        public ValidationRuleSet(IEnumerable<ISpecification> rules)
        {
            _rules = new List<ISpecification>(rules);
        }

        /// <summary>
        /// Returns a new instance that contains both the rules contained in this set and
        /// in the other set.  Does not modify this instance.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public ValidationRuleSet Combine(ValidationRuleSet other)
        {
            List<ISpecification> combined = new List<ISpecification>();
            combined.AddRange(this._rules);
            combined.AddRange(other._rules);

            return new ValidationRuleSet(combined);
        }

        /// <summary>
        /// Provides read-only access to the list of rules contained in this rule set.
        /// </summary>
        public IList<ISpecification> Rules
        {
            get { return _rules.AsReadOnly(); }
        }

        #region ISpecification Members

        /// <summary>
        /// Tests all rules against the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public TestResult Test(object obj)
        {
            return TestCore(obj, null);
        }

        #endregion

        #region IValidationRuleSet Members

        /// <summary>
        /// Tests the subset of rules (those that are selected by the filter) against the specified object.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public TestResult Test(object obj, Predicate<ISpecification> filter)
        {
            return TestCore(obj, filter);
        }

        #endregion

        public TestResult TestCore(object obj, Predicate<ISpecification> filter)
        {
            Platform.CheckForNullReference(obj, "obj");

            // test every specification in the set of rules
            List<TestResultReason> failureReasons = new List<TestResultReason>();
            foreach (ISpecification rule in _rules)
            {
                // if there is no filter, or the fitler accepts the rule, test it
                if (filter == null || filter(rule))
                {
                    // if the rule is itself a ruleset, then apply the filter recursively
                    TestResult result = (rule is IValidationRuleSet) ? (rule as IValidationRuleSet).Test(obj, filter) : rule.Test(obj);
                    if (result.Fail)
                    {
                        failureReasons.AddRange(result.Reasons);
                    }
                }
            }

            return new TestResult(failureReasons.Count == 0, failureReasons.ToArray());
        }
    }
}
