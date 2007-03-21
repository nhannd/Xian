using System;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin.VisitAdmin;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class PlaceOrderRequest : DataContractBase
    {
        public PlaceOrderRequest(
            EntityRef patient, 
            EntityRef visit,
            EntityRef diagnosticService,
            EnumValueInfo orderPriority,
            EntityRef orderingPhysician,
            EntityRef orderingFacility,
            DateTime schedulingRequestTime)
        {
            this.Patient = patient;
            this.Visit = visit;
            this.DiagnosticService = diagnosticService;
            this.OrderPriority = orderPriority;
            this.OrderingPhysician = orderingPhysician;
            this.OrderingFacility = orderingFacility;
            this.SchedulingRequestTime = schedulingRequestTime;
        }

        [DataMember]
        public EntityRef Patient;

        [DataMember]
        public EntityRef Visit;

        [DataMember]
        public EntityRef DiagnosticService;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EntityRef OrderingPhysician;

        [DataMember]
        public EntityRef OrderingFacility;

        [DataMember]
        public DateTime SchedulingRequestTime;
    }
}
