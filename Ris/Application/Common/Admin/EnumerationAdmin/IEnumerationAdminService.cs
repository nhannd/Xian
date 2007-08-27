using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.EnumerationAdmin
{
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
