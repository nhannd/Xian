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
using System.Collections.Generic;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Common;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Reporting
{
    public class WorklistItemKey : IWorklistItemKey
    {
        private EntityRef _reportingProcedureStep;

        public WorklistItemKey(EntityRef reportingProcedureStep)
        {
            _reportingProcedureStep = reportingProcedureStep;
        }

        public EntityRef ReportingProcedureStep
        {
            get { return _reportingProcedureStep; }
            set { _reportingProcedureStep = value; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private string _accessionNumber;
        private OrderPriority _priority;
        private string _requestedProcedureName;
        private string _diagnosticServiceName;
        private ActivityStatus _activityStatus;
        private string _stepType;

        public WorklistItem(
            ReportingProcedureStep reportingProcedureStep,
            CompositeIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority priority,
            string requestedProcedureName,
            string diagnosticServiceName,
            ActivityStatus activityStatus)
            : base(new WorklistItemKey(reportingProcedureStep.GetRef()))
        {
            _mrn = mrn;
            _patientName = patientName;
            _accessionNumber = accessionNumber;
            _priority = priority;
            _requestedProcedureName = requestedProcedureName;
            _diagnosticServiceName = diagnosticServiceName;
            _activityStatus = activityStatus;
            _stepType = reportingProcedureStep.Name;
        }

        #region Public Properties

        public EntityRef ProcedureStepRef
        {
            get { return ((WorklistItemKey)this.Key).ReportingProcedureStep; }
        }

        public CompositeIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public OrderPriority Priority
        {
            get { return _priority; }
        }

        public string RequestedProcedureName
        {
            get { return _requestedProcedureName; }
        }

        public string DiagnosticServiceName
        {
            get { return _diagnosticServiceName; }
        }

        public ActivityStatus ActivityStatus
        {
            get { return _activityStatus; }
        }

        public string StepType
        {
            get { return _stepType; }
        }

        #endregion
    }
}
