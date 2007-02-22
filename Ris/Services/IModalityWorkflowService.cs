using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow.Modality;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IModalityWorkflowService : IHealthcareServiceLayer
    {
        IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria);
        WorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef);
        ModalityProcedureStep LoadWorklistItemPreview(WorklistQueryResult item);

        IDictionary<string, bool> GetOperationEnablement(EntityRef<ModalityProcedureStep> stepRef);
        void ExecuteOperation(EntityRef<ModalityProcedureStep> stepRef, string operationClassName);

    }
}
