using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public abstract class CompositeSpecification : Specification
    {
        private List<ISpecification> _childSpecs = new List<ISpecification>();

        public CompositeSpecification(string testExpression, string failureMessage)
            : base(testExpression, failureMessage)
        {

        }

        public void Add(ISpecification spec)
        {
            _childSpecs.Add(spec);
        }

        public override IEnumerable<ISpecification> SubSpecs
        {
            get { return _childSpecs; }
        }
    }
}
