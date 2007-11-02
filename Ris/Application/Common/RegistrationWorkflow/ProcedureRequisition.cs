using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
    [DataContract]
    public class ProcedureRequisition : DataContractBase
    {
        /// <summary>
        /// Constructor for use by service to return requisition back to client for editing.
        /// </summary>
        public ProcedureRequisition(
            RequestedProcedureTypeSummary procedureType,
            string procedureIndex,
            DateTime? scheduledTime,
            FacilitySummary performingFacility,
            EnumValueInfo laterality,
            bool portableModality,
            EnumValueInfo status,
            bool canModify)
        {
            this.ProcedureType = procedureType;
            this.ProcedureIndex = procedureIndex;
            this.ScheduledTime = scheduledTime;
            this.PerformingFacility = performingFacility;
            this.Laterality = laterality;
            this.PortableModality = portableModality;
            this.Status = status;
            this.CanModify = canModify;
        }

        /// <summary>
        /// Constructor for use by client when initially creating a requisition.
        /// </summary>
        public ProcedureRequisition(RequestedProcedureTypeSummary procedureType, FacilitySummary facility)
        {
            this.ProcedureType = procedureType;
            this.PerformingFacility = facility;
            this.CanModify = true;  // can modify a new requisition
        }


        /// <summary>
        /// The procedure type. Required.
        /// </summary>
        [DataMember]
        public RequestedProcedureTypeSummary ProcedureType;

        /// <summary>
        /// Procedure index.  Will be set by the server if this requisition represents an existing procedure.
        /// Clients should not set or modify this field.
        /// </summary>
        [DataMember]
        public string ProcedureIndex;

        /// <summary>
        /// Time at which this procedure is scheduled to occur. May be null, indicating that
        /// the procedure is not yet scheduled for a specific time.
        /// </summary>
        [DataMember]
        public DateTime? ScheduledTime;

        /// <summary>
        /// Status of this procedure, set by the server.
        /// </summary>
        [DataMember]
        public EnumValueInfo Status;

        /// <summary>
        /// Facility at which this procedure will be performed. Required.
        /// </summary>
        [DataMember]
        public FacilitySummary PerformingFacility;

        /// <summary>
        /// Indicates whether this procedure is to be performed on a portable modality.
        /// </summary>
        [DataMember]
        public bool PortableModality;

        /// <summary>
        /// Laterality for this procedure.
        /// </summary>
        [DataMember]
        public EnumValueInfo Laterality;

        /// <summary>
        /// Set by the server to indicate whether this requested procedure can be modified
        /// during an order modification (e.g. it cannot be modified if it is already in-progress).
        /// </summary>
        [DataMember]
        public bool CanModify;
    }
}
