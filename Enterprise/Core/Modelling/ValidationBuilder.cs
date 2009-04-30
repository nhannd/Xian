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
using ClearCanvas.Common.Specifications;
using System.Reflection;
using ClearCanvas.Common.Utilities;
using System.Collections;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    internal class ValidationBuilder
    {
        public ValidationBuilder()
        {
        }

        public ValidationRuleSet BuildRuleSet(Type entityClass)
        {
            List<ISpecification> rules = new List<ISpecification>();
            ProcessClassProperties(entityClass, rules);

            // process class-level attributes
            foreach (Attribute attr in entityClass.GetCustomAttributes(true))
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
            rules.Add(new UniqueKeySpecification(uka.LogicalName, uka.MemberProperties));
        }

        private void ProcessClassProperties(Type domainClass, List<ISpecification> rules)
        {
            ValidationRuleSet ruleSet = new ValidationRuleSet();

            // note: this will return all properties, including those that are inherited from a base class
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

            if (attr is EmbeddedValueCollectionAttribute)
                ProcessEmbeddedValueCollectionAttribute(property, attr, rules);

            if (attr is UniqueAttribute)
                ProcessUniqueAttribute(property, attr, rules);
        }

        private void ProcessUniqueAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            rules.Add(new UniqueSpecification(property));
        }

        private void ProcessRequiredAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            rules.Add(new RequiredSpecification(property));
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
                rules.Add(new EmbeddedValueRuleSet(property, new ValidationRuleSet(innerRules), false));
            }
        }

        private void ProcessEmbeddedValueCollectionAttribute(PropertyInfo property, Attribute attr, List<ISpecification> rules)
        {
            EmbeddedValueCollectionAttribute ca = (EmbeddedValueCollectionAttribute)attr;

            List<ISpecification> innerRules = new List<ISpecification>();
            ProcessClassProperties(ca.ElementType, innerRules);
            if (innerRules.Count > 0)
            {
                rules.Add(new EmbeddedValueRuleSet(property, new ValidationRuleSet(innerRules), true));
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
