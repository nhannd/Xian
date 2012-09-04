#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class SaveOrderDocumentationDataRequest : DataContractBase
    {
        public SaveOrderDocumentationDataRequest(EntityRef orderRef,
            Dictionary<string, string> orderExtendedProperties,
            List<OrderNoteDetail> orderNotes,
			List<ModalityPerformedProcedureStepDetail> modalityPerformedProcedureSteps,
            StaffSummary assignedInterpreter)
        {
            this.OrderRef = orderRef;
            this.OrderExtendedProperties = orderExtendedProperties;
            this.OrderNotes = orderNotes;
			this.ModalityPerformedProcedureSteps = modalityPerformedProcedureSteps;
            this.AssignedInterpreter = assignedInterpreter;
        }

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public Dictionary<string, string> OrderExtendedProperties;

        [DataMember]
        public List<OrderNoteDetail> OrderNotes;

		[DataMember]
		public List<ModalityPerformedProcedureStepDetail> ModalityPerformedProcedureSteps;

        /// <summary>
        /// Specifies a radiologist to which these procedures should be assigned for interpretation. Optional.
        /// </summary>
        [DataMember]
        public StaffSummary AssignedInterpreter;
    }
}
