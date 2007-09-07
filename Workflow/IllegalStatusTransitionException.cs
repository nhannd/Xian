using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public class IllegalStateTransitionException : WorkflowException
    {
        public IllegalStateTransitionException(string message)
            :base(message)
        {

        }
    }
}
