using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Specifications;
using System.Collections;

namespace ClearCanvas.Healthcare.Workflow
{
    public interface IOperation
    {
        ISpecification InputSpecification { get; }
        void Execute(IWorklistItem item, IList parameters, IWorkflow workflow);
        Staff CurrentUserStaff { get; set; }
    }
}
