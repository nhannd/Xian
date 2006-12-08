using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Specifications
{
    public interface ISpecification
    {
        TestResult Test(object obj);
        IEnumerable<ISpecification> SubSpecs { get; }
    }
}
