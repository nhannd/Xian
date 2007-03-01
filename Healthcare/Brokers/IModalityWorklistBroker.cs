using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Brokers
{
    public interface IModalityWorklistBroker : IPersistenceBroker
    {
        IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria, string patientProfileAuthority);
        WorklistQueryResult GetWorklistItem(EntityRef mpsRef, string patientProfileAuthority);
    }
}
