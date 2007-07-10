using System;
using System.ServiceModel;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [ServiceContract]
    public interface IPreviewService
    {
        [OperationContract]
        GetDataResponse GetData(GetDataRequest request);
    }
}
