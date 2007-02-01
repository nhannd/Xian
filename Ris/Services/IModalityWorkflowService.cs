using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IModalityWorkflowService : IHealthcareServiceLayer
    {
        IList<ModalityWorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria);
        ModalityWorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef);
        ModalityProcedureStep LoadWorklistItemPreview(ModalityWorklistQueryResult item);

        IDictionary<string, bool> GetOperationEnablement(EntityRef<ModalityProcedureStep> stepRef);
        void ExecuteOperation(EntityRef<ModalityProcedureStep> stepRef, string operationClassName);

    }
}
