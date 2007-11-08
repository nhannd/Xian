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

    public class ProtocollingWorklistItemKey : IWorklistItemKey
    {
        private EntityRef _protocollingProcedureStep;

        public ProtocollingWorklistItemKey(EntityRef protocollingProcedureStep)
        {
            _protocollingProcedureStep = protocollingProcedureStep;
        }

        public EntityRef ProtocollingProcedureStep
        {
            get { return _protocollingProcedureStep; }
            set { _protocollingProcedureStep = value; }
        }
    }

    public class WorklistItem : WorklistItemBase
    {
        private EntityRef _profileRef;
        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private string _accessionNumber;
        private OrderPriority _orderPriority;
        private string _requestedProcedureName;
        private string _diagnosticServiceName;
        private ActivityStatus _activityStatus;
        private string _stepType;
        private PatientClassEnum _patientClass;
        private DateTime? _procedureEndTime;

        /// <summary>
        /// Constructor for ReportingWorklistBroker
        /// </summary>
        public WorklistItem(
            ReportingProcedureStep reportingProcedureStep,
            PatientProfile profile,
            string accessionNumber,
            OrderPriority priority,
            string requestedProcedureName,
            string diagnosticServiceName,
            ActivityStatus activityStatus,
            PatientClassEnum patientClass)
            : base(new WorklistItemKey(reportingProcedureStep.GetRef()))
        {
            _profileRef = profile.GetRef();
            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _accessionNumber = accessionNumber;
            _orderPriority = priority;
            _requestedProcedureName = requestedProcedureName;
            _diagnosticServiceName = diagnosticServiceName;
            _activityStatus = activityStatus;
            _stepType = reportingProcedureStep.Name;
            _patientClass = patientClass;
            // _procedureEndTime = ???
        }

        public WorklistItem(
            ProtocolProcedureStep protocolProcedureStep,
            PatientProfile profile,
            string accessionNumber,
            OrderPriority priority,
            string requestedProcedureName,
            string diagnosticServiceName,
            ActivityStatus activityStatus,
            PatientClassEnum patientClass)
            : base(new ProtocollingWorklistItemKey(protocolProcedureStep.GetRef()))
        {
            _profileRef = profile.GetRef();
            _mrn = profile.Mrn;
            _patientName = profile.Name;
            _accessionNumber = accessionNumber;
            _orderPriority = priority;
            _requestedProcedureName = requestedProcedureName;
            _diagnosticServiceName = diagnosticServiceName;
            _activityStatus = activityStatus;
            _stepType = protocolProcedureStep.Name;
            _patientClass = patientClass;
            // _procedureEndTime = 
        }

        #region Public Properties

        public EntityRef ProcedureStepRef
        {
            get
            {
                if(this.Key is WorklistItemKey)
                    return ((WorklistItemKey)this.Key).ReportingProcedureStep;
                else if(this.Key is ProtocollingWorklistItemKey)
                    return ((ProtocollingWorklistItemKey)this.Key).ProtocollingProcedureStep;
                else return null;
            }
        }

        public EntityRef PatientProfileRef
        {
            get { return _profileRef; }
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

        public OrderPriority OrderPriority
        {
            get { return _orderPriority; }
        }

        public PatientClassEnum PatientClass
        {
            get { return _patientClass; }
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

        public DateTime? ProcedureEndTime
        {
            get { return _procedureEndTime; }
        }

        public string StepType
        {
            get { return _stepType; }
        }

        #endregion
    }
}
