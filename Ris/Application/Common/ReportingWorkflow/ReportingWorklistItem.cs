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

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
    [DataContract]
    public class ReportingWorklistItem : WorklistItemSummaryBase
    {
        public ReportingWorklistItem(
            EntityRef procedureStepRef,
            EntityRef procedureRef,
            EntityRef orderRef,
            EntityRef patientRef,
            EntityRef profileRef,
            EntityRef reportRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            string accessionNumber,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string diagnosticServiceName,
            string procedureName,
			bool procedurePortable,
			EnumValueInfo procedureLaterality,
			string procedureStepName,
            DateTime? time,
            EnumValueInfo activityStatus,
			int reportPartIndex)
            : base(
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
            this.ReportRef = reportRef;
            this.ActivityStatus = activityStatus;
        	this.ReportPartIndex = reportPartIndex;
        }

        [DataMember]
        public EntityRef ReportRef;

        [DataMember]
        public EnumValueInfo ActivityStatus;

		[DataMember]
		public int ReportPartIndex;

		/// <summary>
		/// Gets a value indicating if this worklist item refers to an addendum.
		/// </summary>
    	public bool IsAddendumStep
    	{
			get { return this.ReportPartIndex > 0; }
    	}
	}
}
