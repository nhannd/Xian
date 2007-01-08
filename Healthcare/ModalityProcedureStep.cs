using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Workflow;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ModalityProcedureStep entity
    /// </summary>
	public partial class ModalityProcedureStep : ProcedureStep
	{
        private ModalityProcedureStepType _type;
        private Modality _modality;

        public ModalityProcedureStep(RequestedProcedure procedure, ModalityProcedureStepType type, Modality modality)
        {
            this.RequestedProcedure = procedure;
            this.Type = type;
            this.Modality = modality;
            this.Status = ActivityStatus.SC;
        }

        /// <summary>
        /// Default no-args constructor required by NHibernate
        /// </summary>
        public ModalityProcedureStep()
        {
        }

        public virtual ModalityProcedureStepType Type
        {
            get { return _type; }
            set { _type = value; }
        }



        public virtual Modality Modality
        {
            get { return _modality; }
            set { _modality = value; }
        }


        /// <summary>
        /// Starts the scheduled procedure step
        /// </summary>
        public virtual void Start()
        {
            if (!InStatus(new ActivityStatus[] { ActivityStatus.SC }))
                throw new HealthcareWorkflowException("The step has already been started");

            //this.StartTime = Platform.Time;
            this.Status = ActivityStatus.IP;
        }

        /// <summary>
        /// Completes the scheduled procedure step
        /// </summary>
        public virtual void Complete()
        {
            if (InStatus(new ActivityStatus[] {
                ActivityStatus.CM,
                ActivityStatus.DC }))
                throw new HealthcareWorkflowException("Step has already been completed or discontinued");

            //this.EndTime = Platform.Time;
            this.Status = ActivityStatus.CM;

        }

        /// <summary>
        /// Discontinues the scheduled procedure step
        /// </summary>
        public virtual void Discontinue()
        {
            if (InStatus(new ActivityStatus[] {
                ActivityStatus.CM,
                ActivityStatus.DC }))
                throw new HealthcareWorkflowException("Step has already been completed or discontinued");

            //this.EndTime = Platform.Time;
            this.Status = ActivityStatus.DC;
        }

        private bool InStatus(ActivityStatus[] statuses)
        {
            return CollectionUtils.Contains<ActivityStatus>(statuses,
                delegate(ActivityStatus s) { return this.Status == s; });
        }


		#region Object overrides
		
		public override bool Equals(object that)
		{
			// TODO: implement a test for business-key equality
			return base.Equals(that);
		}
		
		public override int GetHashCode()
		{
			// TODO: implement a hash-code based on the business-key used in the Equals() method
			return base.GetHashCode();
		}
		
		#endregion

	}
}