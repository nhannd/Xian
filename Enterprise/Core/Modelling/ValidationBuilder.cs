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
using System.Reflection;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Utility class for building a <see cref="ValidationRuleSet"/> based on attributes defined on an entity class.
	/// </summary>
	internal class ValidationBuilder
	{
		class AttributeEntityClassPair
		{
			public Type EntityClass { get; set; }
			public Attribute Attribute { get; set; }
		}

		#region Public API

		/// <summary>
		/// Builds a set of validation rules by processing the attributes defined on the specified entity class.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <returns></returns>
		public ValidationRuleSet BuildRuleSet(Type entityClass)
		{
			var rules = new List<ISpecification>();
			ProcessClassProperties(entityClass, rules);


			// process class-level attributes
			foreach (var pair in GetClassAttributes(entityClass))
			{
				ProcessEntityAttribute(entityClass, pair, rules);
			}

			return new ValidationRuleSet(rules);
		}

		#endregion

		private static void ProcessEntityAttribute(Type entityClass, AttributeEntityClassPair pair, ICollection<ISpecification> rules)
		{
			// TODO: this could be changed to a dictionary of delegates, or a visitor pattern of some kind

			if (pair.Attribute is UniqueKeyAttribute)
				ProcessUniqueKeyAttribute(entityClass, pair, rules);
		}

		private static void ProcessUniqueKeyAttribute(Type entityClass, AttributeEntityClassPair pair, ICollection<ISpecification> rules)
		{
			var uka = (UniqueKeyAttribute)pair.Attribute;
			rules.Add(new UniqueKeySpecification(pair.EntityClass, uka.LogicalName, uka.MemberProperties));
		}

		private void ProcessClassProperties(Type domainClass, ICollection<ISpecification> rules)
		{
			// note: this will return all properties, including those that are inherited from a base class
			var properties = domainClass.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			foreach (var property in properties)
			{
				foreach (Attribute attr in property.GetCustomAttributes(false))
				{
					ProcessPropertyAttribute(property, attr, rules);
				}
			}
		}

		private void ProcessPropertyAttribute(PropertyInfo property, Attribute attr, ICollection<ISpecification> rules)
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

		private static void ProcessUniqueAttribute(PropertyInfo property, Attribute attr, ICollection<ISpecification> rules)
		{
			rules.Add(new UniqueSpecification(property));
		}

		private static void ProcessRequiredAttribute(PropertyInfo property, Attribute attr, ICollection<ISpecification> rules)
		{
			rules.Add(new RequiredSpecification(property));
		}

		private static void ProcessLengthAttribute(PropertyInfo property, Attribute attr, ICollection<ISpecification> rules)
		{
			CheckAttributeValidOnProperty(attr, property, typeof(string));

			var la = (LengthAttribute)attr;
			rules.Add(new LengthSpecification(property, la.Min, la.Max));
		}

		private void ProcessEmbeddedValueAttribute(PropertyInfo property, Attribute attr, ICollection<ISpecification> rules)
		{
			var innerRules = new List<ISpecification>();
			ProcessClassProperties(property.PropertyType, innerRules);
			if (innerRules.Count > 0)
			{
				rules.Add(new EmbeddedValueRuleSet(property, new ValidationRuleSet(innerRules), false));
			}
		}

		private void ProcessEmbeddedValueCollectionAttribute(PropertyInfo property, Attribute attr, ICollection<ISpecification> rules)
		{
			var ca = (EmbeddedValueCollectionAttribute)attr;

			var innerRules = new List<ISpecification>();
			ProcessClassProperties(ca.ElementType, innerRules);
			if (innerRules.Count > 0)
			{
				rules.Add(new EmbeddedValueRuleSet(property, new ValidationRuleSet(innerRules), true));
			}
		}

		private static void CheckAttributeValidOnProperty(Attribute attr, PropertyInfo property, params Type[] types)
		{
			if (!CollectionUtils.Contains(types, t => t.IsAssignableFrom(property.PropertyType)))
				throw new ModellingException(
					string.Format("{0} attribute cannot be applied to property of type {1}.", attr.GetType().Name, property.PropertyType.FullName));
		}

		private static List<AttributeEntityClassPair> GetClassAttributes(Type entityClass)
		{
			// get attributes on this class only, not on the base class
			var pairs = CollectionUtils.Map(entityClass.GetCustomAttributes(false),
								(Attribute a) => new AttributeEntityClassPair { EntityClass = entityClass, Attribute = a });

			// recur on base class
			var baseClass = entityClass.BaseType;
			if (baseClass != typeof(object))
				pairs.AddRange(GetClassAttributes(baseClass));

			return pairs;
		}
	}
}
