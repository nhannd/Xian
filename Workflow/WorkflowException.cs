using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Workflow
{
    public class WorkflowException : Exception
    {
        public WorkflowException(string message)
            :base(message)
        {

        }

        public WorkflowException(string message, Exception inner)
            :base(message, inner)
        {

        }
    }
}
