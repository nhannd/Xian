using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract base class that roughly represents the notion of an IHE/DICOM General Purpose Scheduled Procedure Step (GP-SPS)
    /// </summary>
    public abstract class ProcedureStep : Activity
    {
        private RequestedProcedure _requestedProcedure;

        public ProcedureStep()
        {
        }

        public ProcedureStep(RequestedProcedure procedure)
        {
            _requestedProcedure = procedure;
        }

        public virtual RequestedProcedure RequestedProcedure
        {
            get { return _requestedProcedure; }
            set { _requestedProcedure = value; }
        }
    }
}
