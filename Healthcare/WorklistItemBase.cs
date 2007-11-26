using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Healthcare
{
    public class WorklistItemBase : IEquatable<WorklistItemBase>
    {
        private readonly EntityRef _procedureStepRef;
        private readonly EntityRef _requestedProcedureRef;
        private readonly EntityRef _orderRef;
        private readonly EntityRef _patientRef;
        private readonly EntityRef _profileRef;


        private readonly PatientIdentifier _mrn;
        private readonly PersonName _patientName;
        private readonly string _accessionNumber;
        private readonly OrderPriority _orderPriority;
        private readonly PatientClassEnum _patientClass;
        private readonly string _diagnosticServiceName;
        private readonly string _requestedProcedureName;
        private readonly string _procedureStepName;
        private readonly DateTime? _scheduledStartTime;


        protected WorklistItemBase()
        {
            //TODO get rid of this constructor
        }

        public WorklistItemBase(
            ProcedureStep procedureStep,
            RequestedProcedure requestedProcedure,
            Order order,
            Patient patient,
            PatientProfile profile,
            PatientIdentifier mrn,
            PersonName patientName,
            string accessionNumber,
            OrderPriority orderPriority,
            PatientClassEnum patientClass,
            string diagnosticServiceName,
            string requestedProcedureName,
            DateTime? scheduledStartTime
            )
        {
            _procedureStepRef = procedureStep == null ? null : procedureStep.GetRef();
            _requestedProcedureRef = requestedProcedure == null ? null : requestedProcedure.GetRef();
            _orderRef = order == null ? null : order.GetRef();
            _patientRef = patient == null ? null : patient.GetRef();
            _profileRef = profile == null ? null : profile.GetRef();
            _mrn = mrn;
            _patientName = patientName;
            _accessionNumber = accessionNumber;
            _orderPriority = orderPriority;
            _patientClass = patientClass;
            _diagnosticServiceName = diagnosticServiceName;
            _requestedProcedureName = requestedProcedureName;
            _procedureStepName = procedureStep == null ? null : procedureStep.Name;
            _scheduledStartTime = scheduledStartTime;
        }


        public EntityRef ProcedureStepRef
        {
            get { return _procedureStepRef; }
        }

        public EntityRef RequestedProcedureRef
        {
            get { return _requestedProcedureRef; }
        }

        public EntityRef OrderRef
        {
            get { return _orderRef; }
        }

        public EntityRef PatientRef
        {
            get { return _patientRef; }
        }

        public EntityRef PatientProfileRef
        {
            get { return _profileRef; }
        }

        public PatientIdentifier Mrn
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

        public string DiagnosticServiceName
        {
            get { return _diagnosticServiceName; }
        }

        public string RequestedProcedureName
        {
            get { return _requestedProcedureName; }
        }

        public string ProcedureStepName
        {
            get { return _procedureStepName; }
        }

        public DateTime? ScheduledStartTime
        {
            get { return _scheduledStartTime; }
        }

        #region object overrides

        /// <summary>
        /// By default, equality is based on procedure step.
        /// </summary>
        /// <param name="worklistItemBase"></param>
        /// <returns></returns>
        public bool Equals(WorklistItemBase worklistItemBase)
        {
            if (worklistItemBase == null) return false;
            return Equals(_procedureStepRef, worklistItemBase._procedureStepRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as WorklistItemBase);
        }

        public override int GetHashCode()
        {
            return _procedureStepRef.GetHashCode();
        }

        #endregion

    }
}
