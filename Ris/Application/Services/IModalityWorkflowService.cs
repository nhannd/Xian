using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Services
{
    public interface IModalityWorkflowService : IHealthcareServiceLayer
    {
        IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria);
        WorklistQueryResult GetWorklistItem(EntityRef mpsRef);
        ModalityProcedureStep LoadWorklistItemPreview(WorklistQueryResult item);

        IDictionary<string, bool> GetOperationEnablement(EntityRef stepRef);
        void ExecuteOperation(EntityRef stepRef, string operationClassName);

    }
}
