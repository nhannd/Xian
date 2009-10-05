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
    public class UniqueKeySpecification : ISpecification
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
        /// might have a unique key consisting of "FirstName" and "LastName" properties.  Note that compound
        /// property expressions may be used, e.g. for a Person class with a Name property that itself has First
        /// and Last properties, the unique key members might be "Name.First" and "Name.Last".
        /// </param>
        public UniqueKeySpecification(Type entityClass, string logicalKeyName, string[] uniqueKeyMembers)
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

        	var domainObj = (DomainObject) obj;
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
