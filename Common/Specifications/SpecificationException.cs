using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class SpecificationException : Exception
    {
        public SpecificationException(string message)
            :base(message)
        {

        }

        public SpecificationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
