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
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProcedureStepDetail : DataContractBase
    {
        public ProcedureStepDetail(
            EntityRef procedureStepRef,
            string procedureStepName,
            string stepClassName,
			string description,
            EnumValueInfo state,
			DateTime? creationTime,
            DateTime? scheduledStartTime,
            DateTime? startTime,
            DateTime? endTime,
            StaffSummary scheduledPerformer,
            StaffSummary performer,
			ModalitySummary modality
            )
        {
            this.ProcedureStepRef = procedureStepRef;
            this.ProcedureStepName = procedureStepName;
            this.StepClassName = stepClassName;
        	this.Description = description;
            this.State = state;
        	this.CreationTime = creationTime;
            this.ScheduledStartTime = scheduledStartTime;
            this.StartTime = startTime;
            this.EndTime = endTime;
			this.Modality = modality;
			this.ScheduledPerformer = scheduledPerformer;
            this.Performer = performer;
        }

		public ProcedureStepDetail()
		{			
		}

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public string ProcedureStepName;

        [DataMember] 
        public string StepClassName;

		[DataMember]
		public string Description;

		[DataMember]
        public EnumValueInfo State;

		[DataMember]
		public DateTime? CreationTime;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public DateTime? StartTime;

        [DataMember]
        public DateTime? EndTime;

        [DataMember]
        public StaffSummary ScheduledPerformer;

        [DataMember]
        public StaffSummary Performer;

		/// <summary>
		/// Specifies the modality of a MPS.  This field is null for other types of procedure step.
		/// </summary>
		[DataMember]
		public ModalitySummary Modality;

    }
}