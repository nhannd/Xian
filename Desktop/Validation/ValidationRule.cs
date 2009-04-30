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
using ClearCanvas.Common;
using ClearCanvas.Common.Specifications;
using System.Xml;

namespace ClearCanvas.Desktop.Validation
{
    /// <summary>
    /// Default implementation of <see cref="IValidationRule"/>.
    /// </summary>
    public class ValidationRule : IValidationRule
    {
		/// <summary>
		/// Delegate used to validate an <see cref="IApplicationComponent"/> and return the 
		/// results as a <see cref="TestResult"/>.
		/// </summary>
        public delegate ValidationResult ValidationDelegate(IApplicationComponent component);

        private string _propertyName;
        private ValidationDelegate _callback;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">The property to which this rule applies.</param>
        /// <param name="callback">A method that performs the validation.</param>
        public ValidationRule(string propertyName, ValidationDelegate callback)
        {
            Platform.CheckForNullReference(propertyName, "propertyName");
            Platform.CheckForNullReference(callback, "callback");

            _propertyName = propertyName;
            _callback = callback;
        }

        /// <summary>
        /// Constructor that accepts an instance of a <see cref="ISpecification"/>.
        /// </summary>
        /// <remarks>
		/// The specification defines the validation rule.
		/// </remarks>
        /// <param name="propertyName">The property to which the rule applies.</param>
        /// <param name="spec">The specification to use to evaluate the rule.</param>
        public ValidationRule(string propertyName, ISpecification spec)
            : this(propertyName, (new SpecificationEvaluator(spec)).Evaluate)
        {
        }

        #region IValidationRule members

        /// <summary>
        /// Gets the name of the property on the application component that this rule applies to.
        /// </summary>
        public string PropertyName
        {
            get { return _propertyName; }
        }

        /// <summary>
        /// Obtains the current result of evaluating this rule based on the current state of the application component.
        /// </summary>
        public ValidationResult GetResult(IApplicationComponent component)
        {
            return _callback(component);
        }

        #endregion

        #region SpecificationEvaluator

        /// <summary>
        /// Helper class for evaluating a specification as a validation rule.
        /// </summary>
        private class SpecificationEvaluator
        {
            private ISpecification _specification;

            public SpecificationEvaluator(ISpecification specification)
            {
                _specification = specification;
            }

            public ValidationResult Evaluate(IApplicationComponent component)
            {
                try
                {
                    TestResult result = _specification.Test(component);
                    return new ValidationResult(result.Success, result.Success ? null : GetTopLevelMessage(result.Reasons));
                }
                catch (Exception e)
                {
                    // if the evaluation of the specification throws an exception, 
                    // treat it as a failed validation and return the exception message
                    // this will assist the administrator in correcting the specification XML
                    return new ValidationResult(false, string.Format("Error evaluating validation rule: {0}", e.Message));
                }
            }

            private static string GetTopLevelMessage(TestResultReason[] reasons)
            {
                if (reasons.Length == 0)
                    return null;

                string message = reasons[0].Message;
                return (!string.IsNullOrEmpty(message)) ? message : GetTopLevelMessage(reasons[0].Reasons);
            }
        }

        #endregion
    }
}
