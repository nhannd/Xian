using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class LoadVisitForAdminEditResponse : DataContractBase
    {
        public LoadVisitForAdminEditResponse(EntityRef visitRef, VisitDetail visitDetail)
        {
            this.VisitRef = visitRef;
            this.VisitDetail = visitDetail;
        }

        [DataMember]
        public EntityRef VisitRef;

        [DataMember]
        public EntityRef Patient;

        [DataMember]
        public VisitDetail VisitDetail;
    }
}
