#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
    [RisApplicationService]
    [ServiceContract]
    public interface IEnumerationAdminService
    {
        [OperationContract]
        ListEnumerationsResponse ListEnumerations(ListEnumerationsRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListEnumerationValuesResponse ListEnumerationValues(ListEnumerationValuesRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        AddValueResponse AddValue(AddValueRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        EditValueResponse EditValue(EditValueRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        RemoveValueResponse RemoveValue(RemoveValueRequest request);
    }
}
