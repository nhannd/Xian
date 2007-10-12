#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class CountSpecification : PrimitiveSpecification
    {
        private int _min = 0;
        private int _max = Int32.MaxValue;

        public CountSpecification(int min, int max)
        {
            _max = max;
            _min = min;
        }

        protected override TestResult InnerTest(object exp, object root)
        {
            if (exp is Array)
            {
                return DefaultTestResult(InRange((exp as Array).Length));
            }

            if (exp is ICollection)
            {
                return DefaultTestResult(InRange((exp as ICollection).Count));
            }

            if (exp is IEnumerable)
            {
                // manually count the items
                // this could be very bad in terms of performance, but let's assume this will rarely
                // happen on a very large collection
                int count = 0;
                foreach (object element in (exp as IEnumerable)) count++;
                return DefaultTestResult(InRange(count));
            }

			throw new SpecificationException(SR.ExceptionCastExpressionArrayCollectionEnumerable);
        }

        protected bool InRange(int n)
        {
            return n >= _min && n <= _max;
        }
    }
}
