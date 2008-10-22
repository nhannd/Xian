#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Runtime.Serialization;
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