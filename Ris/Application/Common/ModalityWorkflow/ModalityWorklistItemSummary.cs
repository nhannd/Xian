#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ModalityWorklistItemSummary : WorklistItemSummaryBase
    {
        public ModalityWorklistItemSummary(
            EntityRef procedureStepRef,
            EntityRef procedureRef,
            EntityRef orderRef,
            EntityRef patientRef,
            EntityRef profileRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            string accessionNumber,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string diagnosticServiceName,
            string procedureName,
			bool procedurePortable,
			EnumValueInfo procedureLaterality,
			string procedureStudyInstanceUID,
            string procedureStepName,
            DateTime? time)
            :base(
                procedureStepRef,
                procedureRef,
                orderRef,
                patientRef,
                profileRef,
                mrn,
                name,
                accessionNumber,
                orderPriority,
                patientClass,
                diagnosticServiceName,
                procedureName,
				procedurePortable,
				procedureLaterality,
                procedureStepName,
                time
            )
        {
        	this.ProcedureStudyInstanceUID = procedureStudyInstanceUID;
        }

		[DataMember]
		public string ProcedureStudyInstanceUID;
	}
}
