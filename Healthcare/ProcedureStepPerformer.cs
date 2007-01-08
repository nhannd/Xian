using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public class ProcedureStepPerformer : ActivityPerformer
    {
        private Staff _staff;

        public Staff Staff
        {
            get { return _staff; }
            set { _staff = value; }
        }
    }
}
