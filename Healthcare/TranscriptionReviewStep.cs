#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// TranscriptionReviewStep entity
	/// </summary>
	public class TranscriptionReviewStep : ClearCanvas.Healthcare.ReportingProcedureStep
	{
		private bool _hasErrors;

		public TranscriptionReviewStep()
		{
		}

		public TranscriptionReviewStep(ReportingProcedureStep previousStep)
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
			get { return "Transcription Review"; }
		}

		protected override ProcedureStep CreateScheduledCopy()
		{
			return new TranscriptionReviewStep(this);
		}

		public virtual bool HasErrors
		{
			get { return _hasErrors; }
			set { _hasErrors = value; }
		}
	}
}