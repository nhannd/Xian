using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;

namespace ClearCanvas.Common.Specifications
{
    public abstract class EnumerableSpecification : Specification
    {
        private ISpecification _elementSpecification;

        public EnumerableSpecification(string testExpression, ISpecification elementSpecification, string failureMessage)
            :base(testExpression, failureMessage)
        {
            _elementSpecification = elementSpecification;
        }

        public EnumerableSpecification(ISpecification elementSpecification)
        {
            _elementSpecification = elementSpecification;
        }

        protected ISpecification ElementSpec
        {
            get { return _elementSpecification; }
        }

        protected IEnumerable AsEnumerable(object obj)
        {
            IEnumerable enumerable = obj as IEnumerable;
            if (enumerable == null)
				throw new SpecificationException(SR.ExceptionCastExpressionEnumerable);

            return enumerable;
        }

        public override IEnumerable<ISpecification> SubSpecs
        {
            get { return new ISpecification[] { _elementSpecification }; }
        }
    }
}
