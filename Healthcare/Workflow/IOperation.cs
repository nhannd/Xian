using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;
using ClearCanvas.Common.Specifications;

namespace ClearCanvas.Healthcare.Workflow
{
    public interface IOperation
    {
        ISpecification InputSpecification { get; }
        void Execute(Activity input, IWorkflow workflow);
        Staff CurrentUserStaff { get; set; }
    }
}
