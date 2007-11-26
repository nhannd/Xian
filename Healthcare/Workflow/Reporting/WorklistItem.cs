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

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;
using System;

namespace ClearCanvas.Healthcare.Workflow.Reporting
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
        private readonly ActivityStatus _activityStatus;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistItem(
            ProcedureStep procedureStep,
            RequestedProcedure requestedProcedure,
            Order order,
            Patient patient,
            PatientProfile profile,
            PatientIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority orderPriority,
            PatientClassEnum patientClass,
            string diagnosticServiceName,
            string requestedProcedureName,
            DateTime? scheduledStartTime,
            ActivityStatus activityStatus)
            : base(
                procedureStep,
                requestedProcedure,
                order,
                patient,
                profile,
                mrn,
                patientName,
                accessionNumber,
                orderPriority,
                patientClass,
                diagnosticServiceName,
                requestedProcedureName,
                scheduledStartTime
            )
        {
            _activityStatus = activityStatus;
        }

        #region Public Properties

        public ActivityStatus ActivityStatus
        {
            get { return _activityStatus; }
        }

        #endregion
    }
}
