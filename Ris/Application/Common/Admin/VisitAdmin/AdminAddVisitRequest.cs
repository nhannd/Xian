using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class AdminAddVisitRequest : DataContractBase
    {
        public AdminAddVisitRequest(VisitDetail visitDetail)
        {
            this.VisitDetail = visitDetail;
        }
        
        [DataMember]
        public VisitDetail VisitDetail;
    }
}
