using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
    [DataContract]
    public class OrderRequisition : DataContractBase
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OrderRequisition()
        {
        }

        /// <summary>
        /// Patient for which procedures are being ordered. Required for new orders. Ignored for order modification.
        /// </summary>
        [DataMember]
        public EntityRef Patient;

        /// <summary>
        /// Visit with which the order is associated. Required.
        /// </summary>
        [DataMember]
        public VisitSummary Visit;

        /// <summary>
        /// Diagnostic service to order. Required for new orders. Ignored for order modification.
        /// </summary>
        [DataMember]
        public DiagnosticServiceSummary DiagnosticService;

        /// <summary>
        /// Reason that the procedures are being ordered. Required.
        /// </summary>
        [DataMember]
        public string ReasonForStudy;

        /// <summary>
        /// Order priority. Required.
        /// </summary>
        [DataMember]
        public EnumValueInfo Priority;

        /// <summary>
        /// The set of procedures being requested. If not provided, the default set of procedures
        /// for the diagnostic service will be ordered.
        /// When modifying an order, existing procedures will be updated from procedures in this list,
        /// and any new procedures in the list will be added to the order.  Any procedure previously
        /// in the order that are not found in the list will be removed from the order.
        /// </summary>
        [DataMember]
        public List<ProcedureRequisition> RequestedProcedures;

        /// <summary>
        /// Facility that is placing the order. Required.
        /// </summary>
        [DataMember]
        public FacilitySummary OrderingFacility;

        /// <summary>
        /// Time that the procedures are requested to be scheduled for, if not actually being scheduled now. Optional.
        /// </summary>
        [DataMember]
        public DateTime? SchedulingRequestTime;

        /// <summary>
        /// Practitioner on behalf of whom the order is being placed. Required.
        /// </summary>
        [DataMember]
        public ExternalPractitionerSummary OrderingPractitioner;

        /// <summary>
        /// Additional practitioners to whom copies of the results should be sent. Optional.
        /// </summary>
        [DataMember]
        public List<ExternalPractitionerSummary> CopiesToPractitioners;
    }
}
