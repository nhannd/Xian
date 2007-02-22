using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow;

namespace ClearCanvas.Ris.Services
{
    public interface IWorklistService : IHealthcareServiceLayer
    {
        IList SearchPatient(PatientProfileSearchCriteria criteria);
        IList GetWorklist(string worklistClassName);
        IList GetWorklist(string worklistClassName, SearchCriteria additionalCriteria);
        IList GetQueryResultForWorklistItem(string worklistClassName, IWorklistItem item);
        //void Subscribe(string worklistClassName, string callback);


        RequestedProcedure LoadRequestedProcedure(EntityRef<RequestedProcedure> rpRef, bool loadDetail);
        void UpdateRequestedProcedure(RequestedProcedure rp);
        void AddCheckInProcedureStep(CheckInProcedureStep cps);

        IWorklistItem LoadWorklistItemPreview(IWorklistQueryResult queryResult);

        IDictionary<string, bool> GetOperationEnablement(EntityRef<ModalityProcedureStep> stepRef);
        void ExecuteOperation(EntityRef<ModalityProcedureStep> stepRef, string operationClassName);
    
    }
}
