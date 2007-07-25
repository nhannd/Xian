using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
    [DataContract]
    public class AddWorklistRequest : DataContractBase
    {
        public AddWorklistRequest(WorklistAdminDetail detail)
        {
            Detail = detail;
        }

        [DataMember]
        public WorklistAdminDetail Detail;
    }
}