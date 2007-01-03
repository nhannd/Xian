using System;
using System.Collections;
using System.Text;

using Iesi.Collections;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;


namespace ClearCanvas.Healthcare {


    /// <summary>
    /// ScheduledProcedureStep entity
    /// </summary>
	public partial class ScheduledProcedureStep : Entity
	{
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        /// <summary>
        /// Starts the scheduled procedure step
        /// </summary>
        public virtual void Start()
        {
            if (!InStatus(new ScheduledProcedureStepStatus[] {ScheduledProcedureStepStatus.SCHEDULED}))
                throw new HealthcareWorkflowException("The step has already been started");    

            this.StartTime = Platform.Time;
            this.Status = ScheduledProcedureStepStatus.INPROGRESS;
        }

        /// <summary>
        /// Completes the scheduled procedure step
        /// </summary>
        public virtual void Complete()
        {
            if (InStatus(new ScheduledProcedureStepStatus[] {
                ScheduledProcedureStepStatus.COMPLETED,
                ScheduledProcedureStepStatus.DISCONTINUED }))
                throw new HealthcareWorkflowException("Step has already been completed or discontinued");

            this.EndTime = Platform.Time;
            this.Status = ScheduledProcedureStepStatus.COMPLETED;
        }

        /// <summary>
        /// Discontinues the scheduled procedure step
        /// </summary>
        public virtual void Discontinue()
        {
            if (InStatus(new ScheduledProcedureStepStatus[] {
                ScheduledProcedureStepStatus.COMPLETED,
                ScheduledProcedureStepStatus.DISCONTINUED }))
                throw new HealthcareWorkflowException("Step has already been completed or discontinued");

            this.EndTime = Platform.Time;
            this.Status = ScheduledProcedureStepStatus.DISCONTINUED;
        }

        private bool InStatus(ScheduledProcedureStepStatus[] statuses)
        {
            return CollectionUtils.Contains<ScheduledProcedureStepStatus>(statuses,
                delegate(ScheduledProcedureStepStatus s) { return this.Status == s; });
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