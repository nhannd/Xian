#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Specifies that one or more properties of an entity form a unique key for that entity.
	/// </summary>
	/// <remarks>
	/// Internally, this class makes use of a <see cref="IUniqueConstraintValidationBroker"/> to validate
	/// that the key is unique within the space of entities of a given class.
	/// </remarks>
	internal class UniqueKeySpecification : ISpecification
	{
		private readonly Type _entityClass;
		private readonly string[] _uniqueKeyMembers;
		private readonly string _logicalKeyName;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="entityClass">Class on which the unique key constraint is defined.</param>
		/// <param name="logicalKeyName">The logical name of the key.  This value is used in reporting failures,
		/// and will be used as a key into the string resources.
		/// </param>
		/// <param name="uniqueKeyMembers">
		/// An array of property names that form the unique key for the class.  For example, a Person class
		/// might have a unique key consisting of "FirstName" and "LastName" properties.  Compound
		/// property expressions may be used, e.g. for a Person class with a Name property that itself has First
		/// and Last properties, the unique key members might be "Name.First" and "Name.Last".
		/// </param>
		internal UniqueKeySpecification(Type entityClass, string logicalKeyName, string[] uniqueKeyMembers)
		{
			_entityClass = entityClass;
			_uniqueKeyMembers = uniqueKeyMembers;
			_logicalKeyName = logicalKeyName;
		}

		public TestResult Test(object obj)
		{
			var context = PersistenceScope.CurrentContext;
			if (context == null)
				throw new SpecificationException(SR.ExceptionPersistenceContextRequired);

			var domainObj = (DomainObject)obj;
			var broker = context.GetBroker<IUniqueConstraintValidationBroker>();
			var valid = broker.IsUnique(domainObj, _entityClass, _uniqueKeyMembers);

			return valid ? new TestResult(true) : new TestResult(false, new TestResultReason(GetMessage(domainObj)));
		}

		protected virtual string GetMessage(DomainObject obj)
		{
			return string.Format(SR.RuleUniqueKey, TerminologyTranslator.Translate(obj.GetClass(), _logicalKeyName),
				TerminologyTranslator.Translate(_entityClass));
		}
	}
}
