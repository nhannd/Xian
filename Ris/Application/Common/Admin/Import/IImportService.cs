using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.Admin.Import
{
    [ServiceContract]
    public interface IImportService
    {
        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        ListImportersResponse ListImporters(ListImportersRequest request);

        [OperationContract]
        [FaultContract(typeof(RequestValidationException))]
        [FaultContract(typeof(ImportException))]
        ImportDataResponse ImportData(ImportDataRequest request);
    }
}
