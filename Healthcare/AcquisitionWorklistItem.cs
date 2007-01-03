using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Healthcare
{
    public class AcquisitionWorklistItem
    {
        private EntityRef<Patient> _patient;
        private EntityRef<PatientProfile> _patientProfile;
        private EntityRef<Order> _order;
        private EntityRef<RequestedProcedure> _requestedProcedure;
        private EntityRef<ScheduledProcedureStep> _workflowStep;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private CompositeIdentifier _visitNumber;
        private string _accessionNumber;
        private string _diagnosticService;
        private string _procedure;
        private string _scheduledStep;
        private string _modality;
        private OrderPriority _priority;
        private ScheduledProcedureStepStatus _status;

        public AcquisitionWorklistItem(
            Patient patient,
            PatientProfile profile,
            Order order,
            RequestedProcedure requestedProcedure,
            ScheduledProcedureStep workflowStep,
            CompositeIdentifier mrn,
            PersonName patientName,
            CompositeIdentifier visitNumber,
            string accessionNumber,
            string diagnosticService,
            string procedure,
            string scheduledStep,
            string modality,
            OrderPriority priority,
            ScheduledProcedureStepStatus status)
        {
            _patient = new EntityRef<Patient>(patient);
            _patientProfile = new EntityRef<PatientProfile>(profile);
            _order = new EntityRef<Order>(order);
            _requestedProcedure = new EntityRef<RequestedProcedure>(requestedProcedure);
            _workflowStep = new EntityRef<ScheduledProcedureStep>(workflowStep);

            _mrn = mrn;
            _patientName = patientName;
            _visitNumber = visitNumber;
            _accessionNumber = accessionNumber;
            _diagnosticService = diagnosticService;
            _procedure = procedure;
            _scheduledStep = scheduledStep;
            _modality = modality;
            _priority = priority;
            _status = status;
        }

        public EntityRef<ScheduledProcedureStep> WorkflowStep
        {
            get { return _workflowStep; }
        }

        public EntityRef<Patient> Patient
        {
            get { return _patient; }
        }

        public EntityRef<PatientProfile> PatientProfile
        {
            get { return _patientProfile; }
        }

        public EntityRef<Order> Order
        {
            get { return _order; }
        }

        public EntityRef<RequestedProcedure> RequestedProcedure
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
            get { return _diagnosticService; }
        }
        public string Procedure
        {
            get { return _procedure; }
        }
        public string ScheduledStep
        {
            get { return _scheduledStep; }
        }
        public string Modality
        {
            get { return _modality; }
        }

        public OrderPriority Priority
        {
            get { return _priority; }
        }

        public ScheduledProcedureStepStatus Status
        {
            get { return _status; }
        }
    }
}
