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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public class WorklistItemKey
    {
        private readonly EntityRef _procedureStepRef;

        public WorklistItemKey(EntityRef procedureStepRef)
        {
            _procedureStepRef = procedureStepRef;
        }

        public EntityRef ProcedureStepRef
        {
            get { return _procedureStepRef; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
		/// <summary>
		/// Constructor for patient search result item.
		/// </summary>
		/// <param name="patient"></param>
		/// <param name="profile"></param>
		/// <param name="mrn"></param>
		/// <param name="patientName"></param>
		public WorklistItem(
			Patient patient,
			PatientProfile profile,
			PatientIdentifier mrn,
			PersonName patientName)
			: base(
			null,
			null,
			null,
			patient,
			profile,
			mrn,
			patientName,
			null,
			Healthcare.OrderPriority.R,// technically this should be null, but we don't have that option because its a value type
			null,
			null,
			null,
			false,// technically this should be null, but we don't have that option because its a value type
			Laterality.N,// technically this should be null, but we don't have that option because its a value type
			null)
		{
		}

		/// <summary>
		/// Constructor for procedure search result item.
		/// </summary>
		public WorklistItem(
			Procedure procedure,
			Order order,
			Patient patient,
			PatientProfile profile,
			PatientIdentifier mrn,
			PersonName patientName,
			string accessionNumber,
			OrderPriority orderPriority,
			PatientClassEnum patientClass,
			string diagnosticServiceName,
			string procedureName,
			bool procedurePortable,
			Laterality procedureLaterality,
			DateTime? time)
			: base(
			null,
			procedure,
			order,
			patient,
			profile,
			mrn,
			patientName,
			accessionNumber,
			orderPriority,
			patientClass,
			diagnosticServiceName,
			procedureName,
			procedurePortable,
			procedureLaterality,
			time)
		{

		}

		/// <summary>
        /// Constructor for worklist item.
        /// </summary>
        public WorklistItem(
            ProcedureStep procedureStep,
            Procedure procedure,
            Order order,
            Patient patient,
            PatientProfile profile,
            PatientIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority orderPriority,
            PatientClassEnum patientClass,
            string diagnosticServiceName,
            string procedureName,
			bool procedurePortable,
			Laterality procedureLaterality,
            DateTime? time)
            : base(
                procedureStep,
                procedure,
                order,
                patient,
                profile,
                mrn,
                patientName,
                accessionNumber,
                orderPriority,
                patientClass,
                diagnosticServiceName,
                procedureName,
				procedurePortable, 
				procedureLaterality,
                time
            )
        {
        }
    }
}
