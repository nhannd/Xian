using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [ServiceContract]
    public interface IOrderEntryService
    {
        [OperationContract]
        ListActiveVisitsForPatientResponse ListActiveVisitsForPatient(ListActiveVisitsForPatientRequest request);

        [OperationContract]
        GetOrderEntryFormDataResponse GetOrderEntryFormData(GetOrderEntryFormDataRequest request);

        [OperationContract]
        LoadDiagnosticServiceBreakdownResponse LoadDiagnosticServiceBreakdown(LoadDiagnosticServiceBreakdownRequest request);

        [OperationContract]
        PlaceOrderResponse PlaceOrder(PlaceOrderRequest request);

        [OperationContract]
        GetOrdersWorkListResponse GetOrdersWorkList(GetOrdersWorkListRequest request);
    }
}
