using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public class WorklistQueryResult : IWorklistQueryResult
    {
        private EntityRef _patient;
        private EntityRef _patientProfile;
        private EntityRef _order;
        private EntityRef _requestedProcedure;
        private EntityRef _procedureStep;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private CompositeIdentifier _visitNumber;
        private string _accessionNumber;
        private string _diagnosticServiceName;
        private string _requestedProcedureName;
        private string _procedureStepName;
        private string _modalityName;
        private OrderPriority _priority;
        private ActivityStatus _status;

        public WorklistQueryResult(
            Patient patient,
            PatientProfile profile,
            Order order,
            RequestedProcedure requestedProcedure,
            ModalityProcedureStep procedureStep,
            CompositeIdentifier mrn,
            PersonName patientName,
            CompositeIdentifier visitNumber,
            string accessionNumber,
            string diagnosticService,
            string requestedProcedureName,
            string procedureStepName,
            string modalityName,
            OrderPriority priority,
            ActivityStatus status)
        {
            _patient = patient.GetRef();
            _patientProfile = profile.GetRef();
            _order = order.GetRef();
            _requestedProcedure = requestedProcedure.GetRef();
            _procedureStep = procedureStep.GetRef();

            _mrn = mrn;
            _patientName = patientName;
            _visitNumber = visitNumber;
            _accessionNumber = accessionNumber;
            _diagnosticServiceName = diagnosticService;
            _requestedProcedureName = requestedProcedureName;
            _procedureStepName = procedureStepName;
            _modalityName = modalityName;
            _priority = priority;
            _status = status;
        }

        public override bool Equals(object obj)
        {
            WorklistQueryResult that = (WorklistQueryResult)obj;
            return that != null && this._procedureStep.Equals(that._procedureStep);
        }

        public override int GetHashCode()
        {
            return _procedureStep.GetHashCode();
        }

        public EntityRef ProcedureStep
        {
            get { return _procedureStep; }
        }

        public EntityRef Patient
        {
            get { return _patient; }
        }

        public EntityRef PatientProfile
        {
            get { return _patientProfile; }
        }

        public EntityRef Order
        {
            get { return _order; }
        }

        public EntityRef RequestedProcedure
        {
            get { return _requestedProcedure; }
        }

        public CompositeIdentifier Mrn
        {
            get { return _mrn; }
        }

        public PersonName PatientName
        {
            get { return _patientName; }
        }

        public CompositeIdentifier VisitNumber
        {
            get { return _visitNumber; }
        }

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public string DiagnosticService
        {
            get { return _diagnosticServiceName; }
        }

        public string RequestedProcedureName
        {
            get { return _requestedProcedureName; }
        }

        public string ModalityProcedureStepName
        {
            get { return _procedureStepName; }
        }

        public string ModalityName
        {
            get { return _modalityName; }
        }

        public OrderPriority Priority
        {
            get { return _priority; }
        }

        public ActivityStatus Status
        {
            get { return _status; }
        }
    }
}
