#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    public interface IWorklistService
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
