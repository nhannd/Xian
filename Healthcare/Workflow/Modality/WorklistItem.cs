using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare.Workflow.Modality
{
    public class WorklistItemKey : IWorklistItemKey
    {
        private EntityRef _procedureStep;

        public WorklistItemKey(EntityRef procedureStep)
        {
            _procedureStep = procedureStep;
        }

        public EntityRef ProcedureStep
        {
            get { return _procedureStep; }
            set { _procedureStep = value; }
        }
    }

    //TODO:  This need to be trim down to contain only fields that is in the ModalityWorklist Pane
    public class WorklistItem : WorklistItemBase
    {
        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private string _accessionNumber;
        private ModalityProcedureStepType _modalityProcedureStepType;
        private RequestedProcedureType _requestedProcedureType;
        private OrderPriority _priority;
        private ClearCanvas.Healthcare.Modality _modality;

        //public WorklistItem(
        //    ModalityProcedureStep modalityProcedureStep,
        //    PatientProfile profile,
        //    RequestedProcedure requestedProcedure)
        //    : base(new WorklistItemKey(modalityProcedureStep.GetRef()))
        //{
        //    _mrn = profile.Mrn;
        //    _patientName = profile.Name;
        //    _accessionNumber = requestedProcedure.Order.AccessionNumber;
        //    _priority = requestedProcedure.Order.Priority;
        //    _requestedProcedureType = requestedProcedure.Type;
        //    _modalityProcedureStepType = modalityProcedureStep.Type;
        //    _modality = modalityProcedureStep.Modality;
        //}

        //public WorklistItem(ModalityProcedureStep modalityProcedureStep)
        //    : base(new WorklistItemKey(modalityProcedureStep.GetRef()))
        //{
        //    _mrn = new CompositeIdentifier("MRN", "MRNAA");
        //    _patientName = new PersonName("FAMILY", "GIVEN");
        //    _accessionNumber = "ACCESSIONNUMBER";
        //    _priority = OrderPriority.A;
        //    _requestedProcedureType = null;
        //    _modalityProcedureStepType = modalityProcedureStep.Type;
        //    _modality = modalityProcedureStep.Modality;
        //}

        public WorklistItem(
            ModalityProcedureStep modalityProcedureStep,
            CompositeIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority priority,
            RequestedProcedureType requestedProcedureType,
            ModalityProcedureStepType modalityProcedureStepType,
            ClearCanvas.Healthcare.Modality modality)
            : base(new WorklistItemKey(modalityProcedureStep.GetRef()))
        {
            _mrn = mrn;
            _patientName = patientName;
            _accessionNumber = accessionNumber;
            _priority = priority;
            _requestedProcedureType = requestedProcedureType;
            _modalityProcedureStepType = modalityProcedureStepType;
            _modality = modality;
        }


        public EntityRef ModalityProcedureStepRef
        {
            get { return (this.Key as WorklistItemKey).ProcedureStep; }
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

        public RequestedProcedureType RequestedProcedureType
        {
            get { return _requestedProcedureType; }
        }

        public ModalityProcedureStepType ModalityProcedureStepType
        {
            get { return _modalityProcedureStepType; }
        }

        public ClearCanvas.Healthcare.Modality Modality
        {
            get { return _modality; }
        }

        //private EntityRef _patient;
        //private EntityRef _patientProfile;
        //private EntityRef _order;
        //private EntityRef _requestedProcedure;

        //private CompositeIdentifier _mrn;
        //private PersonName _patientName;
        //private CompositeIdentifier _visitNumber;
        //private string _accessionNumber;
        //private string _diagnosticServiceName;
        //private string _requestedProcedureName;
        //private string _procedureStepName;
        //private string _modalityName;
        //private OrderPriority _priority;
        //private ActivityStatus _status;

        //public WorklistItem(
        //    Patient patient,
        //    PatientProfile profile,
        //    Order order,
        //    RequestedProcedure requestedProcedure,
        //    ModalityProcedureStep procedureStep,
        //    CompositeIdentifier mrn,
        //    PersonName patientName,
        //    CompositeIdentifier visitNumber,
        //    string accessionNumber,
        //    string diagnosticService,
        //    string requestedProcedureName,
        //    string procedureStepName,
        //    string modalityName,
        //    OrderPriority priority,
        //    ActivityStatus status)
        //    : base(new WorklistItemKey(procedureStep.GetRef()))
        //{
        //    _patient = patient.GetRef();
        //    _patientProfile = profile.GetRef();
        //    _order = order.GetRef();
        //    _requestedProcedure = requestedProcedure.GetRef();

        //    _mrn = mrn;
        //    _patientName = patientName;
        //    _visitNumber = visitNumber;
        //    _accessionNumber = accessionNumber;
        //    _diagnosticServiceName = diagnosticService;
        //    _requestedProcedureName = requestedProcedureName;
        //    _procedureStepName = procedureStepName;
        //    _modalityName = modalityName;
        //    _priority = priority;
        //    _status = status;
        //}

        //public EntityRef ProcedureStep
        //{
        //    get { return (this.Key as WorklistItemKey).ProcedureStep; }
        //}

        //public EntityRef Patient
        //{
        //    get { return _patient; }
        //}

        //public EntityRef PatientProfile
        //{
        //    get { return _patientProfile; }
        //}

        //public EntityRef Order
        //{
        //    get { return _order; }
        //}

        //public EntityRef RequestedProcedure
        //{
        //    get { return _requestedProcedure; }
        //}

        //public CompositeIdentifier Mrn
        //{
        //    get { return _mrn; }
        //}

        //public PersonName PatientName
        //{
        //    get { return _patientName; }
        //}

        //public CompositeIdentifier VisitNumber
        //{
        //    get { return _visitNumber; }
        //}

        //public string AccessionNumber
        //{
        //    get { return _accessionNumber; }
        //}

        //public string DiagnosticService
        //{
        //    get { return _diagnosticServiceName; }
        //}

        //public string RequestedProcedureName
        //{
        //    get { return _requestedProcedureName; }
        //}

        //public string ModalityProcedureStepName
        //{
        //    get { return _procedureStepName; }
        //}

        //public string ModalityName
        //{
        //    get { return _modalityName; }
        //}

        //public OrderPriority Priority
        //{
        //    get { return _priority; }
        //}

        //public ActivityStatus Status
        //{
        //    get { return _status; }
        //}
    }
}
