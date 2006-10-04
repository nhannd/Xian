using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace ClearCanvas.Desktop.Validation
{
    public static class ValidationAttributeProcessor
    {
        public static IDictionary<string, ValidatorGroup> Process(object target)
        {
            Dictionary<string, ValidatorGroup> groups = new Dictionary<string, ValidatorGroup>();

            foreach (PropertyInfo propInfo in target.GetType().GetProperties())
            {
                object[] validationAttributes = propInfo.GetCustomAttributes(typeof(ValidationAttribute), false);
                if (validationAttributes.Length > 0)
                {
                    MethodInfo getMethod = propInfo.GetGetMethod();

                    TestValueCallbackDelegate getter = (TestValueCallbackDelegate)
                        Delegate.CreateDelegate(typeof(TestValueCallbackDelegate), target, getMethod);

                    List<IValidator> validators = new List<IValidator>();
                    foreach (ValidationAttribute attr in validationAttributes)
                    {
                        validators.Add(attr.CreateValidator(getter));
                    }
                    groups.Add(propInfo.Name, new ValidatorGroup(validators));
                }
            }
            return groups;
        }
    }
}
