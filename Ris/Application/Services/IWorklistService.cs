using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Healthcare.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Services
{
    public interface IWorklistService : IHealthcareServiceLayer
    {
        IList GetWorklist(string worklistClassName);
        IList GetWorklist(string worklistClassName, SearchCriteria additionalCriteria);
        IList GetQueryResultForWorklistItem(string worklistClassName, IWorklistItem item);
        //void Subscribe(string worklistClassName, string callback);


        RequestedProcedure LoadRequestedProcedure(EntityRef rpRef, bool loadDetail);
        void UpdateRequestedProcedure(RequestedProcedure rp);
        void AddCheckInProcedureStep(CheckInProcedureStep cps);

        IWorklistItem LoadWorklistItemPreview(IWorklistQueryResult queryResult);

        IDictionary<string, bool> GetOperationEnablement(EntityRef stepRef);
        void ExecuteOperation(EntityRef stepRef, string operationClassName);
    
    }
}
