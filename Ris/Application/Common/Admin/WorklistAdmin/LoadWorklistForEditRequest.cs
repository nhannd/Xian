using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class LoadWorklistForEditRequest : DataContractBase
    {
        public LoadWorklistForEditRequest(EntityRef entityRef)
        {
            EntityRef = entityRef;
        }

        [DataMember]
        public EntityRef EntityRef;
    }
}