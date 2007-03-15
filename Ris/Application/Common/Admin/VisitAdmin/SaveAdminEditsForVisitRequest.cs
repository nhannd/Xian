using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class SaveAdminEditsForVisitRequest : DataContractBase
    {
        [DataMember]
        public EntityRef VisitRef;

        [DataMember]
        public VisitDetail VisitDetail;
    }
}
