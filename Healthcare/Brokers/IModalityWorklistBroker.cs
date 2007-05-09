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
        IList<WorklistItem> GetWorklist(string worklistClassName);
        IList<WorklistItem> GetWorklist(ModalityProcedureStepSearchCriteria criteria, string patientProfileAuthority);

        WorklistItem GetWorklistItem(string worklistClassName);
        WorklistItem GetWorklistItem(EntityRef mpsRef, string patientProfileAuthority);

        int GetWorklistCount(string WorklistClassName);
    }
}
