using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Base class for all workflow-related exceptions
    /// </summary>
    public class HealthcareWorkflowException : Exception
    {
        public HealthcareWorkflowException(string message, Exception inner)
            :base(message, inner)
        {
        }

        public HealthcareWorkflowException(string message)
            :base(message)
        {
        }
    }
}
