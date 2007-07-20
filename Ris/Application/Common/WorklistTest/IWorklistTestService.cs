using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

namespace ClearCanvas.Ris.Application.Common.WorklistTest
{
    [ServiceContract]
    public interface IWorklistTestService
    {
        [OperationContract]
        GetAWorklistResponse GetAWorklist(GetAWorklistRequest request);

        [OperationContract]
        DoWorklistTestResponse DoWorklistTest(DoWorklistTestRequest request);
    }

    [DataContract]
    public class GetAWorklistRequest : DataContractBase
    {
    }

    [DataContract]
    public class GetAWorklistResponse : DataContractBase
    {
        public GetAWorklistResponse(WorklistSummary summary)
        {
            Summary = summary;
        }

        [DataMember]
        public WorklistSummary Summary;
    }

    [DataContract]
    public class DoWorklistTestRequest : DataContractBase
    {
        public DoWorklistTestRequest(EntityRef worklistRef)
        {
            WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;
    }

    [DataContract]
    public class DoWorklistTestResponse : DataContractBase
    {
        public DoWorklistTestResponse(string message)
        {
            Message = message;
        }

        [DataMember]
        public string Message;
    }
}
