using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    public class WorklistItem
    {
        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private CompositeIdentifier _visitNumber;
        private string _accessionNumber;
        private string _diagnosticService;
        private string _procedure;
        private string _scheduledStep;
        private string _modality;
        private OrderPriority _priority;

        public WorklistItem(
            CompositeIdentifier mrn,
            PersonName patientName,
            CompositeIdentifier visitNumber,
            string accessionNumber,
            string diagnosticService,
            string procedure,
            string scheduledStep,
            string modality,
            OrderPriority priority)
        {
            _mrn = mrn;
            _patientName = patientName;
            _visitNumber = visitNumber;
            _accessionNumber = accessionNumber;
            _diagnosticService = diagnosticService;
            _procedure = procedure;
            _scheduledStep = scheduledStep;
            _modality = modality;
            _priority = priority;
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

    }
}
