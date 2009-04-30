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
using System.Reflection;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// Specifies minimum and maximum allowable length of the value of a given string-typed property of an object.
    /// </summary>
    public class LengthSpecification : SimpleInvariantSpecification
    {
        private int _min;
        private int _max;

        public LengthSpecification(PropertyInfo property, int min, int max)
            :base(property)
        {
            _min = min;
            _max = max;
        }

        public override TestResult Test(object obj)
        {
            object value = GetPropertyValue(obj);

            // ignore null values
            if (value == null)
                return new TestResult(true);

            try
            {
                string text = (string)value;

                return text.Length >= _min && text.Length <= _max ? new TestResult(true) :
                    new TestResult(false, new TestResultReason(GetMessage()));

            }
            catch (InvalidCastException e)
            {
                throw new SpecificationException(SR.ExceptionExpectedStringValue, e);
            }
        }

        private string GetMessage()
        {
            return string.Format(SR.RuleLength,
                TerminologyTranslator.Translate(this.Property), _min, _max);
        }
    }
}
