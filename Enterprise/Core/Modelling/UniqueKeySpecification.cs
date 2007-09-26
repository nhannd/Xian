using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public class UniqueKeySpecification : ISpecification
    {
        private string[] _uniqueKeyMembers;
        private string _logicalKeyName;

        public UniqueKeySpecification(string[] uniqueKeyMembers, string logicalKeyName)
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
