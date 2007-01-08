using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Abstract class that roughly represents the notion of an IHE/DICOM General Purpose Performed Procedure Step (GP-PPS)
    /// </summary>
    public abstract class PerformedProcedureStep : ActivityPerformedStep
    {
        public PerformedProcedureStep()
        {
        }
    }
}
