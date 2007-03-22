using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.VisitAdmin
{
    [DataContract]
    public class AdminAddVisitResponse : DataContractBase
    {
        public AdminAddVisitResponse(VisitSummary addedVisit)
        {
            this.AddedVisit = addedVisit;
        }

        [DataMember]
        public VisitSummary AddedVisit;
    }
}
