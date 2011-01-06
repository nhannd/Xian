#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
			public Type DeclaringClass { get; set; }
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

			if (pair.Attribute is ValidationAttribute)
				ProcessValidationSupportAttribute(entityClass, pair, rules);
		}

		private static void ProcessValidationSupportAttribute(Type entityClass, AttributeEntityClassPair pair, ICollection<ISpecification> rules)
		{
			// check if the attribute specifies a method for retrieving additional rules
			var a = (ValidationAttribute)pair.Attribute;
			if (string.IsNullOrEmpty(a.HighLevelRulesProviderMethod))
				return;

			// find method on class (use the class that declared the attribute, not the entityClass)
			const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
			var method = pair.DeclaringClass.GetMethod(a.HighLevelRulesProviderMethod, bindingFlags);

			// validate method signature
			if (method == null)
				throw new InvalidOperationException(string.Format("Method {0} not found on class {1}", a.HighLevelRulesProviderMethod, pair.DeclaringClass.FullName));
			if (method.GetParameters().Length != 0 || !typeof(IValidationRuleSet).IsAssignableFrom(method.ReturnType))
				throw new InvalidOperationException(string.Format("Method {0} must have 0 parameters and return IValidationRuleSet", a.HighLevelRulesProviderMethod));

			var ruleSet = (IValidationRuleSet)method.Invoke(null, null);

			rules.Add(ruleSet);
		}

		private static void ProcessUniqueKeyAttribute(Type entityClass, AttributeEntityClassPair pair, ICollection<ISpecification> rules)
		{
			var uka = (UniqueKeyAttribute)pair.Attribute;
			rules.Add(new UniqueKeySpecification(pair.DeclaringClass, uka.LogicalName, uka.MemberProperties));
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
			// get attributes on this class only - do not get inherited attributes, since these will be handled by recursion below
			var pairs = CollectionUtils.Map(entityClass.GetCustomAttributes(false),
								(Attribute a) => new AttributeEntityClassPair { DeclaringClass = entityClass, Attribute = a });

			// recur on base class
			var baseClass = entityClass.BaseType;
			if (baseClass != typeof(object))
				pairs.AddRange(GetClassAttributes(baseClass));

			return pairs;
		}
	}
}
