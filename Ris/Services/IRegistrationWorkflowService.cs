using System;
using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Services
{
    public interface IRegistrationWorkflowService : IHealthcareServiceLayer
    {
        IList<RegistrationWorklistQueryResult> GetWorklist(ModalityProcedureStepSearchCriteria criteria);
        RegistrationWorklistQueryResult GetWorklistItem(EntityRef<ModalityProcedureStep> mpsRef);
        ModalityProcedureStep LoadWorklistItemPreview(RegistrationWorklistQueryResult item);

        IDictionary<string, bool> GetOperationEnablement(EntityRef<ModalityProcedureStep> stepRef);
        void ExecuteOperation(EntityRef<ModalityProcedureStep> stepRef, string operationClassName);

        ModalityProcedureStep LoadStep(EntityRef<ModalityProcedureStep> stepRef);
    }
}
