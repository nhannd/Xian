using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class NotEqualSpecification : ComparisonSpecification
    {
        protected override bool CompareValues(object testValue, object refValue)
        {
            return !object.Equals(testValue, refValue);
        }
    }
}
