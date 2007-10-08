using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public class LessThanSpecification : InequalitySpecification
    {
        public LessThanSpecification()
            :base(-1)
        {
        }
    }
}
