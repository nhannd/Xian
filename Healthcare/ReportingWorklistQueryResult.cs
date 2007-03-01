using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Enterprise.Core;
using ClearCanvas.Workflow;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    public class ReportingWorklistQueryResult
    {
        private EntityRef _patient;
        private EntityRef _patientProfile;
        private EntityRef _order;
        private EntityRef _requestedProcedure;
        private EntityRef _procedureStep;

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
            _patient = patient.GetRef();
            _patientProfile = profile.GetRef();
            _order = order.GetRef();
            _requestedProcedure = requestedProcedure.GetRef();
            _procedureStep = procedureStep.GetRef();

            _mrn = mrn;
            _patientName = patientName;
            _accessionNumber = accessionNumber;
            _diagnosticServiceName = diagnosticService;
            _requestedProcedureName = requestedProcedureName;
            _priority = priority;
            _status = status;
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
