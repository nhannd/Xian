#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{


    /// <summary>
    /// VerificationStep entity
    /// </summary>
	public partial class VerificationStep : ReportingProcedureStep
	{

        public VerificationStep(ReportingProcedureStep previousStep)
			: base(previousStep)
        {
        }
	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}

        public override string Name
        {
            get { return "Verification"; }
        }

        protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
        {
            if (newState == ActivityStatus.CM)
                this.ReportPart.Verifier = this.PerformingStaff;

            base.OnStateChanged(previousState, newState);
        }

		public override ProcedureStep Reassign(Staff performer)
		{
			VerificationStep reassign = base.Reassign(performer).Downcast<VerificationStep>();

			// When reassigning a verification step to another staff, we should reassign the supervisor as well
			// so the report part will be reviewed by the appropriate staff radiologist
			if (reassign.ReportPart != null && reassign.ReportPart.Supervisor != null)
				reassign.ReportPart.Supervisor = performer;

			return reassign;
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new VerificationStep(this);
		}
	}
}