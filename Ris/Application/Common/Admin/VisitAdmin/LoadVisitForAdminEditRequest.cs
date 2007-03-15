using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class LoadVisitForAdminEditRequest : DataContractBase
    {
        public LoadVisitForAdminEditRequest(EntityRef visitRef)
        {
            this.VisitRef = visitRef;
        }

        [DataMember]
        public EntityRef VisitRef;
    }
}
