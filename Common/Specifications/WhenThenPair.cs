using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    internal class WhenThenPair
    {
        private readonly ISpecification _when;
        private readonly ISpecification _then;

        public WhenThenPair(ISpecification when, ISpecification then)
        {
            _when = when;
            _then = then;
        }

        public ISpecification When
        {
            get { return _when; }
        }

        public ISpecification Then
        {
            get { return _then; }
        }
    }
}
