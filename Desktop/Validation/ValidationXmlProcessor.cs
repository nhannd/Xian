using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.IO;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Desktop.Validation
{
    public static class ValidationXmlProcessor
    {
        /*
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
        */

        public static void ProcessXml(ApplicationComponent target)
        {
            ResourceResolver rr = new ResourceResolver(target.GetType().Assembly);
            string resourceName = string.Format("{0}.val.xml", target.GetType().Name);
            try
            {
                using (Stream xmlStream = rr.OpenResource(resourceName))
                {
                    SpecificationFactory specFactory = new SpecificationFactory(xmlStream);
                    IDictionary<string, ISpecification> specs = specFactory.GetAllSpecifications();
                    foreach (KeyValuePair<string, ISpecification> kvp in specs)
                    {
                        target.Validation.Add(new ValidationRule(kvp.Key, kvp.Value));
                    }
                }
            }
            catch (Exception)
            {
                // no cfg file for this component
            }
        }
    }
}
