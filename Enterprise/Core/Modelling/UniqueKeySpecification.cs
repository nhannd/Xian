using System;
using System.Collections.Generic;
using System.Text;
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
        private string[] _uniqueKeyMembers;
        private string _logicalKeyName;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logicalKeyName">The logical name of the key.  This value is used in reporting failures,
        /// and will be used as a key into the string resources.
        /// </param>
        /// <param name="uniqueKeyMembers">
        /// An array of property names that form the unique key for the class.  For example, a Person class
        /// might have a unique key consisting of "FirstName" and "LastName" properties.  Note that compound
        /// property expressions may be used, e.g. for a Person class with a Name property that itself has First
        /// and Last properties, the unique key members might be "Name.First" and "Name.Last".
        /// </param>
        public UniqueKeySpecification(string logicalKeyName, string[] uniqueKeyMembers)
        {
            _uniqueKeyMembers = uniqueKeyMembers;
            _logicalKeyName = logicalKeyName;
        }

        public TestResult Test(object obj)
        {
            IPersistenceContext context = PersistenceScope.Current;
            if (context == null)
                throw new SpecificationException(SR.ExceptionPersistenceContextRequired);

            IUniqueConstraintValidationBroker broker = context.GetBroker<IUniqueConstraintValidationBroker>();
            bool valid = broker.IsUnique((DomainObject)obj, _uniqueKeyMembers);

            return valid ? new TestResult(true) : new TestResult(false, new TestResultReason(GetMessage(obj)));
        }

        protected virtual string GetMessage(object obj)
        {
            return string.Format(SR.RuleUniqueKey, TerminologyTranslator.Translate(obj.GetType(), _logicalKeyName),
                TerminologyTranslator.Translate(obj.GetType()));
        }
    }
}
