#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Represents the result of a particular auditable event.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventResult"/> has a 1-to-1 mapping with a <see cref="EventIdentificationContentsEventOutcomeIndicator"/>,
	/// but <see cref="AuditHelper"/> uses <see cref="EventResult"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventResult
	{
		/// <summary>
		/// The auditable event completed successfully.
		/// </summary>
		public static readonly EventResult Success = new EventResult(EventIdentificationContentsEventOutcomeIndicator.Success);

		/// <summary>
		/// The auditable event finished with minor errors.
		/// </summary>
		public static readonly EventResult MinorFailure = new EventResult(EventIdentificationContentsEventOutcomeIndicator.MinorFailureActionRestarted);

		/// <summary>
		/// The auditable event finished with major errors.
		/// </summary>
		public static readonly EventResult MajorFailure = new EventResult(EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable);

		/// <summary>
		/// The auditable event finished with serious errors.
		/// </summary>
		public static readonly EventResult SeriousFailure = new EventResult(EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated);

		private readonly EventIdentificationContentsEventOutcomeIndicator _outcome;

		private EventResult(EventIdentificationContentsEventOutcomeIndicator outcome)
		{
			_outcome = outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventIdentificationContentsEventOutcomeIndicator"/>.
		/// </summary>
		public static implicit operator EventIdentificationContentsEventOutcomeIndicator(EventResult operand)
		{
			return operand._outcome;
		}

		/// <summary>
		/// Converts the <paramref name="operand"/> to the equivalent <see cref="EventResult"/>.
		/// </summary>
		public static implicit operator EventResult(EventIdentificationContentsEventOutcomeIndicator operand)
		{
			switch (operand)
			{
				case EventIdentificationContentsEventOutcomeIndicator.Success:
					return Success;
				case EventIdentificationContentsEventOutcomeIndicator.MinorFailureActionRestarted:
					return MinorFailure;
				case EventIdentificationContentsEventOutcomeIndicator.SeriousFailureActionTerminated:
					return SeriousFailure;
				case EventIdentificationContentsEventOutcomeIndicator.MajorFailureActionMadeUnavailable:
					return MajorFailure;
				default:
					return null;
			}
		}
	}
}