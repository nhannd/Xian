using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class OrderDetail : DataContractBase
    {
        public OrderDetail()
        {
            this.RequestedProcedures = new List<RequestedProcedureSummary>();
        }

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public VisitDetail Visit;

        [DataMember]
        public string PlacerNumber;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public DiagnosticServiceDetail DiagnosticService;

        [DataMember]
        public DateTime? EnteredDateTime;

        [DataMember]
        public DateTime? SchedulingRequestDateTime;

        [DataMember]
        public PractitionerDetail OrderingPractitioner;

        [DataMember]
        public FacilityDetail OrderingFacility;

        [DataMember]
        public string ReasonForStudy;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo CancelReason;

        [DataMember]
        public List<RequestedProcedureSummary> RequestedProcedures;

    }
}
