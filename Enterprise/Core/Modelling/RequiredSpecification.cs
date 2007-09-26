using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public class RequiredSpecification : SimpleInvariantSpecification
    {
        public RequiredSpecification(PropertyInfo property)
            :base(property)
        {
        }

        public override TestResult Test(object obj)
        {
            object value = GetPropertyValue(obj);

            // special consideration for strings - empty string should be considered "null"
            bool isNull = (value is string) ? string.IsNullOrEmpty((string)value) : value == null;

            return isNull ? new TestResult(false, new TestResultReason(GetMessage())) : new TestResult(true);
        }

        private string GetMessage()
        {
            return string.Format(SR.RuleRequired, TerminologyTranslator.Translate(this.Property));
        }
    }
}
