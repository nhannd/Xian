#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class ModalityWorklistItem : DataContractBase
    {
        public ModalityWorklistItem(
            EntityRef patientRef,
            EntityRef profileRef,
            EntityRef orderRef,
            EntityRef procedureStepRef,
            CompositeIdentifierDetail mrn,
            PersonNameDetail name,
            EnumValueInfo orderPriority,
            EnumValueInfo patientClass,
            string accessionNumber,
            string requestedProcedureName,
            string modalityProcedureStepName,
            string modalityName,
            DateTime? scheduledStartTime,
            string diagnosticServiceName)
        {
            this.PatientRef = patientRef;
            this.PatientProfileRef = profileRef;
            this.OrderRef = orderRef;
            this.ProcedureStepRef = procedureStepRef;
            this.Mrn = mrn;
            this.PersonNameDetail = name;
            this.OrderPriority = orderPriority;
            this.PatientClass = patientClass;
            this.AccessionNumber = accessionNumber;
            this.RequestedProcedureTypeName = requestedProcedureName;
            this.ModalityProcedureStepName = modalityProcedureStepName;
            this.ModalityName = modalityName;
            this.ScheduledStartTime = scheduledStartTime;
            this.DiagnosticServiceName = diagnosticServiceName;
        }

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public CompositeIdentifierDetail Mrn;

        [DataMember]
        public PersonNameDetail PersonNameDetail;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo PatientClass;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public string ModalityProcedureStepName;

        [DataMember]
        public string RequestedProcedureTypeName;

        [DataMember]
        public string ModalityName;

        [DataMember]
        public DateTime? ScheduledStartTime;

        [DataMember]
        public string DiagnosticServiceName;

        public override bool Equals(object obj)
        {
            ModalityWorklistItem that = obj as ModalityWorklistItem;
            if (that != null)
                return Equals(this.ProcedureStepRef, that.ProcedureStepRef);

            return false;
        }

        public override int GetHashCode()
        {
            return this.ProcedureStepRef.GetHashCode();
        }
    }
}
