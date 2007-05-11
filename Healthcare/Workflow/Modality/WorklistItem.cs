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

    public class WorklistItem : WorklistItemBase
    {
        private CompositeIdentifier _mrn;
        private PersonName _patientName;
        private string _accessionNumber;
        private ModalityProcedureStepType _modalityProcedureStepType;
        private RequestedProcedureType _requestedProcedureType;
        private OrderPriority _priority;
        private ClearCanvas.Healthcare.Modality _modality;

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
    }
}
