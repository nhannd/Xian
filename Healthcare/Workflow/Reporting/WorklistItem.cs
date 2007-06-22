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
