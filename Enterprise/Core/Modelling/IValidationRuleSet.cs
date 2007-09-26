using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    public interface IValidationRuleSet : ISpecification
    {
        TestResult Test(object obj, Predicate<ISpecification> filter);
    }
}
