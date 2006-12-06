using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Desktop.Validation
{
    public static class ValidationAttributeProcessor
    {
        public static List<IValidationRule> Process(object target)
        {
            List<IValidationRule> validators = new List<IValidationRule>();
            foreach (PropertyInfo propInfo in target.GetType().GetProperties())
            {
                object[] validationAttributes = propInfo.GetCustomAttributes(typeof(ValidationAttribute), false);
                if (validationAttributes.Length > 0)
                {
                    MethodInfo getMethod = propInfo.GetGetMethod();

                    TestValueCallbackDelegate getter = (TestValueCallbackDelegate)
                        Delegate.CreateDelegate(typeof(TestValueCallbackDelegate), target, getMethod);

                    foreach (ValidationAttribute attr in validationAttributes)
                    {
                        validators.Add(attr.CreateRule(propInfo.Name, getter));
                    }
                }
            }
            return validators;
        }
    }
}
