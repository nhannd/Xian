using System;
using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Modality
{
    public interface IModalityWorkflowService
    {
        IList<WorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria);
        WorklistQueryResult GetWorklistItem(EntityRef mpsRef);
        ModalityProcedureStep LoadWorklistItemPreview(WorklistQueryResult item);

        IDictionary<string, bool> GetOperationEnablement(EntityRef stepRef);
        void ExecuteOperation(EntityRef stepRef, string operationClassName);

    }
}
