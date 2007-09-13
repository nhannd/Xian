using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class ReplaceOrderRequest : DataContractBase
    {
        public ReplaceOrderRequest(
            EntityRef patient, 
            EntityRef visit,
            EntityRef diagnosticService,
            EnumValueInfo orderPriority,
            EntityRef orderingPhysician,
            EntityRef orderingFacility,
            bool scheduleOrder,
            DateTime schedulingRequestTime,
            EntityRef cancelOrderRef,
            EnumValueInfo reOrderReason)
        {
            this.Patient = patient;
            this.Visit = visit;
            this.DiagnosticService = diagnosticService;
            this.OrderPriority = orderPriority;
            this.OrderingPhysician = orderingPhysician;
            this.OrderingFacility = orderingFacility;
            this.ScheduleOrder = scheduleOrder;
            this.SchedulingRequestTime = schedulingRequestTime;
            this.CancelOrderRef = cancelOrderRef;
            this.ReOrderReason = reOrderReason;
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
        public bool ScheduleOrder;

        [DataMember]
        public DateTime SchedulingRequestTime;

        [DataMember]
        public EntityRef CancelOrderRef;

        [DataMember]
        public EnumValueInfo ReOrderReason;
    }
}
