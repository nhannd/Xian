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
    public class WorklistItemSummaryBase : DataContractBase, IVersionedEquatable<WorklistItemSummaryBase>
    {
        public WorklistItemSummaryBase(
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
            string procedureStepName,
            DateTime? time)
        {
            this.ProcedureStepRef = procedureStepRef;
            this.ProcedureRef = procedureRef;
            this.OrderRef = orderRef;
            this.PatientRef = patientRef;
            this.PatientProfileRef = profileRef;
            this.Mrn = mrn;
            this.PatientName = name;
            this.AccessionNumber = accessionNumber;
            this.OrderPriority = orderPriority;
            this.PatientClass = patientClass;
            this.DiagnosticServiceName = diagnosticServiceName;
            this.ProcedureName = procedureName;
        	this.ProcedurePortable = procedurePortable;
        	this.ProcedureLaterality = procedureLaterality;
            this.ProcedureStepName = procedureStepName;
            this.Time = time;
        }

		public WorklistItemSummaryBase()
		{
		}

        [DataMember]
        public EntityRef ProcedureStepRef;

        [DataMember]
        public EntityRef ProcedureRef;

        [DataMember]
        public EntityRef OrderRef;

        [DataMember]
        public EntityRef PatientRef;

        [DataMember]
        public EntityRef PatientProfileRef;

        [DataMember]
        public CompositeIdentifierDetail Mrn;

        [DataMember]
        public PersonNameDetail PatientName;

        [DataMember]
        public string AccessionNumber;

        [DataMember]
        public EnumValueInfo OrderPriority;

        [DataMember]
        public EnumValueInfo PatientClass;

        [DataMember]
        public string DiagnosticServiceName;

        [DataMember]
        public string ProcedureName;

		[DataMember]
		public bool ProcedurePortable;

		[DataMember]
		public EnumValueInfo ProcedureLaterality;

		[DataMember]
        public string ProcedureStepName;

        [DataMember]
        public DateTime? Time;

		/// <summary>
		/// Implements equality based on all entity-refs, and is version-sensitive.
		/// </summary>
		/// <param name="worklistItemSummaryBase"></param>
		/// <returns></returns>
    	public bool Equals(WorklistItemSummaryBase worklistItemSummaryBase)
    	{
			return Equals(worklistItemSummaryBase, false);
    	}

		/// <summary>
		/// Overridden to provide equality based on all entity-refs.
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
    	public override bool Equals(object obj)
    	{
    		if (ReferenceEquals(this, obj)) return true;
    		return Equals(obj as WorklistItemSummaryBase);
    	}

		/// <summary>
		/// Overridden to provide hash-code based on all entity refs.
		/// </summary>
		/// <returns></returns>
    	public override int GetHashCode()
    	{
    		int result = ProcedureStepRef != null ? ProcedureStepRef.GetHashCode() : 0;
    		result = 29*result + (ProcedureRef != null ? ProcedureRef.GetHashCode() : 0);
    		result = 29*result + (OrderRef != null ? OrderRef.GetHashCode() : 0);
    		result = 29*result + PatientRef.GetHashCode();
    		result = 29*result + PatientProfileRef.GetHashCode();
    		return result;
    	}

		#region IVersionedEquatable<WorklistItemSummaryBase> Members

		public bool Equals(WorklistItemSummaryBase other, bool ignoreVersion)
		{
			if (other == null) return false;
			if (!EntityRef.Equals(ProcedureStepRef, other.ProcedureStepRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(ProcedureRef, other.ProcedureRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(OrderRef, other.OrderRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(PatientRef, other.PatientRef, ignoreVersion)) return false;
			if (!EntityRef.Equals(PatientProfileRef, other.PatientProfileRef, ignoreVersion)) return false;
			return true;
		}

		#endregion

		#region IVersionedEquatable Members

		public bool Equals(object other, bool ignoreVersion)
		{
			return Equals(other as WorklistItemSummaryBase, ignoreVersion);
		}

		#endregion
	}
}
