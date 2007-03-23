using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class SaveAdminEditsForVisitRequest : DataContractBase
    {
        public SaveAdminEditsForVisitRequest(EntityRef visitRef, VisitDetail detail)
        {
            this.VisitRef = visitRef;
            this.VisitDetail = detail;
        }

        [DataMember]
        public EntityRef VisitRef;

        [DataMember]
        public VisitDetail VisitDetail;
    }
}
