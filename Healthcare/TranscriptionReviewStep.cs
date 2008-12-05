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