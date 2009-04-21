using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Represents the result of a particular auditable event.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventResult"/> has a 1-to-1 mapping with a <see cref="EventIdentificationTypeEventOutcomeIndicator"/>,
	/// but <see cref="AuditHelper"/> uses <see cref="EventResult"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventResult
	{
		/// <summary>
		/// The auditable event completed successfully.
		/// </summary>
		public static readonly EventResult Success = new EventResult(EventIdentificationTypeEventOutcomeIndicator.Success);

		/// <summary>
		/// The auditable event finished with minor errors.
		/// </summary>
		public static readonly EventResult MinorFailure = new EventResult(EventIdentificationTypeEventOutcomeIndicator.MinorFailureActionRestarted);

		/// <summary>
		/// The auditable event finished with major errors.
		/// </summary>
		public static readonly EventResult MajorFailure = new EventResult(EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable);

		/// <summary>
		/// The auditable event finished with serious errors.
		/// </summary>
		public static readonly EventResult SeriousFailure = new EventResult(EventIdentificationTypeEventOutcomeIndicator.SeriousFailureActionTerminated);

		private readonly EventIdentificationTypeEventOutcomeIndicator _outcome;

		private EventResult(EventIdentificationTypeEventOutcomeIndicator outcome)
		{
			_outcome = outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventIdentificationTypeEventOutcomeIndicator"/>.
		/// </summary>
		public static implicit operator EventIdentificationTypeEventOutcomeIndicator(EventResult operand)
		{
			return operand._outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventResult"/>.
		/// </summary>
		public static implicit operator EventResult(EventIdentificationTypeEventOutcomeIndicator operand)
		{
			switch (operand)
			{
				case EventIdentificationTypeEventOutcomeIndicator.Success:
					return Success;
				case EventIdentificationTypeEventOutcomeIndicator.MinorFailureActionRestarted:
					return MinorFailure;
				case EventIdentificationTypeEventOutcomeIndicator.SeriousFailureActionTerminated:
					return SeriousFailure;
				case EventIdentificationTypeEventOutcomeIndicator.MajorFailureActionMadeUnavailable:
					return MajorFailure;
				default:
					return null;
			}
		}
	}
}