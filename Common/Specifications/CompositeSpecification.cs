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

        public CompositeSpecification()
        {

        }

        public void Add(ISpecification spec)
        {
            _childSpecs.Add(spec);
        }

        public void AddRange(IEnumerable<ISpecification> specs)
        {
            _childSpecs.AddRange(specs);
        }

        public override IEnumerable<ISpecification> SubSpecs
        {
            get { return _childSpecs; }
        }

        public bool IsEmpty
        {
            get { return _childSpecs.Count == 0; }
        }
    }
}
