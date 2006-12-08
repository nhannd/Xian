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

        public static List<IValidationRule> ProcessXml(IApplicationComponent target)
        {
            try
            {
                ResourceResolver rr = new ResourceResolver(target.GetType().Assembly);
                using (Stream s = rr.OpenResource("validation.xml"))
                {
                    SpecificationFactory specFactory = new SpecificationFactory(s);
                    List<IValidationRule> validators = new List<IValidationRule>();
                    foreach (PropertyInfo propInfo in target.GetType().GetProperties())
                    {
                        try
                        {
                            ISpecification spec = specFactory.GetSpecification(propInfo.Name);
                            validators.Add(new SpecValidationRule(target, propInfo.Name, spec));
                        }
                        catch (UndefinedSpecificationException)
                        {
                            // ignore
                        }
                    }
                    return validators;
                }

            }
            catch(Exception)
            {
                // ignore this for now, since we haven't set everything up properly yet
                return new List<IValidationRule>();
            }
        }


    }
}
