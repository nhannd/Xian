using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public class ValidationBuilder
    {
        class EmbeddedValueSpecification : ISpecification, IPropertyBoundRule
        {
            private ValidationRuleSet _innerRules;
            private PropertyInfo _property;

            public EmbeddedValueSpecification(PropertyInfo property, ValidationRuleSet innerRules)
            {
                _property = property;
                _innerRules = innerRules;
            }

            #region ISpecification Members

            public TestResult Test(object obj)
            {
                object propertyValue = _property.GetGetMethod().Invoke(obj, null);

                // if the propertyValue is null, return true
                // this seems counter-intuitive, but what we are effectively saying is that the rules
                // are bound to the propertyValue being tested - if there is no propertyValue, there are no rules to test
                if (propertyValue == null)
                    return new TestResult(true);

                TestResult result = _innerRules.Test(propertyValue);
                return result.Success ?
                    result : new TestResult(false, new TestResultReason(_property.Name + " is invalid.", result.Reasons));
            }

            #endregion

            #region IPropertyBoundRule Members

            public PropertyInfo[] Properties
            {
                get { return new PropertyInfo[] { _property }; }
            }

            #endregion
        }


        public ValidationBuilder()
        {
        }

        public ValidationRuleSet BuildRuleSet(Type entityClass)
        {
            List<ISpecification> rules = new List<ISpecification>();
            ProcessClassProperties(entityClass, rules);

            // process class-level attributes
            foreach (Attribute attr in entityClass.GetCustomAttributes(false))
            {
                ProcessEntityAttribute(entityClass, attr, rules);
            }

            return new ValidationRuleSet(rules);
        }

        private void ProcessEntityAttribute(Type entityClass, Attribute attr, List<ISpecification> rules)
        {
            // TODO: this could be changed to a dictionary of delegates, or a visitor pattern of some kind

            if (attr is UniqueKeyAttribute)
                ProcessUniqueKeyAttribute(entityClass, attr, rules);
        }

        private void ProcessUniqueKeyAttribute(Type entityClass, Attribute attr, List<ISpecification> rules)
        {
            UniqueKeyAttribute uka = (UniqueKeyAttribute)attr;
            rules.Add(new UniqueKeySpecification(uka.MemberProperties, uka.LogicalName));
        }

        private void ProcessClassProperties(Type domainClass, List<ISpecification> rules)
        {
            ValidationRuleSet ruleSet = new ValidationRuleSet();
            PropertyInfo[] properties = domainClass.GetProperties(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance);
            foreach(PropertyInfo property in properties)
            {
                foreach (Attribute attr in property.GetCustomAttributes(false))
                {
                    ProcessPropertyAttribute(property, attr, rules);
                }
            }
        }

        private void ProcessPropertyAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            // TODO: this could be changed to a dictionary of delegates, or a visitor pattern of some kind

            if (attr is RequiredAttribute)
                ProcessRequiredAttribute(property, attr, rules);

            if (attr is LengthAttribute)
                ProcessLengthAttribute(property, attr, rules);

            if (attr is EmbeddedValueAttribute)
                ProcessEmbeddedValueAttribute(property, attr, rules);

            if (attr is UniqueAttribute)
                ProcessUniqueAttribute(property, attr, rules);
        }

        private void ProcessUniqueAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            rules.Add(new UniqueSpecification(property));
        }

        private void ProcessRequiredAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            RequiredAttribute ra = (RequiredAttribute)attr;
            if (ra.IsRequired)
            {
                rules.Add(new RequiredSpecification(property));
            }
        }

        private void ProcessLengthAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            CheckAttributeValidOnProperty(attr, property, typeof(string));

            LengthAttribute la = (LengthAttribute)attr;
            rules.Add(new LengthSpecification(property, la.Min, la.Max));
        }

        private void ProcessEmbeddedValueAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {

            List<ISpecification> innerRules = new List<ISpecification>();
            ProcessClassProperties(property.PropertyType, innerRules);
            if (innerRules.Count > 0)
            {
                rules.Add(new EmbeddedValueSpecification(property, new ValidationRuleSet(innerRules)));
            }
        }

        private void CheckAttributeValidOnProperty(Attribute attr, PropertyInfo property, params Type[] types)
        {
            if (!CollectionUtils.Contains<Type>(types, delegate(Type t) { return t.IsAssignableFrom(property.PropertyType); }))
                throw new ModellingException(
                    string.Format("{0} attribute cannot be applied to property of type {1}.", attr.GetType().Name, property.PropertyType.FullName));
        }
    }
}
