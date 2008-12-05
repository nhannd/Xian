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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;
using System;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class WorklistItemKey
    {
        private readonly EntityRef _procedureStepRef;
        private readonly EntityRef _procedureRef;

        public WorklistItemKey(EntityRef procedureStepRef, EntityRef procedureRef)
        {
            _procedureStepRef = procedureStepRef;
            _procedureRef = procedureRef;
        }

        public EntityRef ProcedureStepRef
        {
            get { return _procedureStepRef; }
        }

        public EntityRef ProcedureRef
        {
            get { return _procedureRef; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        private readonly ActivityStatus _activityStatus;
        private readonly EntityRef _reportRef;
        private readonly int _reportPartIndex = -1;
        private readonly bool _hasErrors;

        /// <summary>
        /// Constructor for protocol item (no report)
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
            bool hasErrors,
            Laterality procedureLaterality,
            DateTime? time,
            ActivityStatus activityStatus)
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
            _activityStatus = activityStatus;
            _hasErrors = hasErrors;
        }

        /// <summary>
        /// Constructor for reporting item (with report)
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
            DateTime? scheduledStartTime,
            ActivityStatus activityStatus,
            Report report,
            ReportPart reportPart)
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
                scheduledStartTime
            )
        {
            _reportRef = report == null ? null : report.GetRef();
            _activityStatus = activityStatus;
            _reportPartIndex = reportPart == null ? -1 : reportPart.Index;
        }

		/// <summary>
		/// Constructor for reporting item (with report and hasErrors flag)
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
			DateTime? scheduledStartTime,
			ActivityStatus activityStatus,
			Report report,
			ReportPart reportPart,
			bool hasErrors)
			: this(
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
				scheduledStartTime,
				activityStatus,
				report,
				reportPart)
		{
			_hasErrors = hasErrors;
		}

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
            :base(
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

        #region Public Properties

        public EntityRef ReportRef
        {
            get { return _reportRef; }
        }

        public ActivityStatus ActivityStatus
        {
            get { return _activityStatus; }
        }

        /// <summary>
        /// Gets the report part index, or -1 if there is no report part.
        /// </summary>
        public int ReportPartIndex
        {
            get { return _reportPartIndex; }
        }

        public bool HasErrors
        {
            get { return _hasErrors; }
        }

        #endregion
    }
}
