using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Desktop.Validation
{
    public static class ValidationAttributeProcessor
    {
        public static List<IValidator> Process(object target)
        {
            List<IValidator> validators = new List<IValidator>();
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
                        validators.Add(attr.CreateValidator(propInfo.Name, getter));
                    }
                }
            }
            return validators;
        }
    }
}
