using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    public class ReportingWorklistQueryResult
    {
        private EntityRef<Patient> _patient;
        private EntityRef<PatientProfile> _patientProfile;
        private EntityRef<Order> _order;
        private EntityRef<RequestedProcedure> _requestedProcedure;
        private EntityRef<ReportingProcedureStep> _procedureStep;

        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private string _accessionNumber;
        private string _diagnosticServiceName;
        private string _requestedProcedureName;
        private OrderPriority _priority;
        private ActivityStatus _status;

        public ReportingWorklistQueryResult(
            Patient patient,
            PatientProfile profile,
            Order order,
            RequestedProcedure requestedProcedure,
            ReportingProcedureStep procedureStep,
            CompositeIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            string diagnosticService,
            string requestedProcedureName,
            OrderPriority priority,
            ActivityStatus status)
        {
            _patient = new EntityRef<Patient>(patient);
            _patientProfile = new EntityRef<PatientProfile>(profile);
            _order = new EntityRef<Order>(order);
            _requestedProcedure = new EntityRef<RequestedProcedure>(requestedProcedure);
            _procedureStep = new EntityRef<ReportingProcedureStep>(procedureStep);

            _mrn = mrn;
            _patientName = patientName;
            _accessionNumber = accessionNumber;
            _diagnosticServiceName = diagnosticService;
            _requestedProcedureName = requestedProcedureName;
            _priority = priority;
            _status = status;
        }


        public EntityRef<ReportingProcedureStep> ProcedureStep
        {
            get { return _procedureStep; }
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

        public string AccessionNumber
        {
            get { return _accessionNumber; }
        }

        public string DiagnosticServiceName
        {
            get { return _diagnosticServiceName; }
        }

        public string RequestedProcedureName
        {
            get { return _requestedProcedureName; }
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
