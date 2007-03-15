using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class SaveAdminEditsForVisitResponse : DataContractBase
    {
        [DataMember]
        public VisitSummary AddedVisit;
    }
}
