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
	/// TranscriptionStep entity
	/// </summary>
	public partial class TranscriptionStep : ReportingProcedureStep
	{

		public TranscriptionStep(ReportingProcedureStep previousStep)
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
			get { return "Transcription"; }
		}

		protected override void OnStateChanged(ActivityStatus previousState, ActivityStatus newState)
		{
			if (newState == ActivityStatus.CM)
			{
				if (this.ReportPart == null)
					throw new WorkflowException("This ReportingStep does not have an associated ReportPart.");

				// When a supervisor completes a submitted transcription, do not overwrite the original transcriber.
				if (this.ReportPart.Transcriber == null)
					this.ReportPart.Transcriber = this.PerformingStaff;
			}

			base.OnStateChanged(previousState, newState);
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new TranscriptionStep(this);
		}
	}
}
