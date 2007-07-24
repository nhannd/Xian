using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Application.Common.WorklistTest
{
    [ServiceContract]
    public interface IWorklistTestService
    {
        [OperationContract]
        GetAWorklistResponse GetAWorklist(GetAWorklistRequest request);

        [OperationContract]
        DoWorklistTestResponse DoWorklistTest(DoWorklistTestRequest request);

        [OperationContract]
        GetPersistentWorklistResponse GetPersistentWorklist(GetPersistentWorklistRequest request);

        [OperationContract]
        GetPersistentWorklistCountResponse GetPersistentWorklistCount(GetPersistentWorklistCountRequest request);
    }

    [DataContract]
    public class GetPersistentWorklistCountRequest : DataContractBase
    {
        public GetPersistentWorklistCountRequest(EntityRef worklistRef)
        {
            WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;
    }

    [DataContract]
    public class GetPersistentWorklistCountResponse : DataContractBase
    {
        public GetPersistentWorklistCountResponse(int itemCount)
        {
            ItemCount = itemCount;
        }

        [DataMember]
        public int ItemCount;
    }

    [DataContract]
    public class GetPersistentWorklistRequest : DataContractBase
    {
        public GetPersistentWorklistRequest(EntityRef worklistRef)
        {
            WorklistRef = worklistRef;
        }

        [DataMember]
        public EntityRef WorklistRef;
    }

    [DataContract]
    public class GetPersistentWorklistResponse : DataContractBase
    {
        public GetPersistentWorklistResponse(List<RegistrationWorklistItem> worklistItems)
        {
            WorklistItems = worklistItems;
        }

        [DataMember]
        public List<RegistrationWorklistItem> WorklistItems;
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
