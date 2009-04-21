using ClearCanvas.Dicom.Audit;

namespace ClearCanvas.ImageViewer.Services.Auditing
{
	/// <summary>
	/// Represents the action taken by the application entity upon receiving a transfer of DICOM instances.
	/// </summary>
	/// <remarks>
	/// In actuality, each <see cref="EventReceiptAction"/> has a 1-to-1 mapping with a <see cref="EventIdentificationTypeEventActionCode"/>,
	/// but <see cref="AuditHelper"/> uses <see cref="EventReceiptAction"/> to abstract away any requirement for knowledge of the
	/// underlying audit types defined in the DICOM toolkit.
	/// </remarks>
	public sealed class EventReceiptAction
	{
		/// <summary>
		/// The device does not already have these instances, and hence created new ones.
		/// </summary>
		public static readonly EventReceiptAction CreateNew = new EventReceiptAction(EventIdentificationTypeEventActionCode.C);

		/// <summary>
		/// The device already has these instances, has determined them to be no different from the arriving ones, and hence did not perform any action.
		/// </summary>
		public static readonly EventReceiptAction KeepExisting = new EventReceiptAction(EventIdentificationTypeEventActionCode.R);

		/// <summary>
		/// The device already has these instances, has determined them to be different from the arriving ones, and hence updated the existing ones.
		/// </summary>
		public static readonly EventReceiptAction UpdateExisting = new EventReceiptAction(EventIdentificationTypeEventActionCode.U);

		/// <summary>
		/// The action that the receiving device took is unknown.
		/// </summary>
		public static readonly EventReceiptAction ActionUnknown = new EventReceiptAction(EventIdentificationTypeEventActionCode.E);

		private readonly EventIdentificationTypeEventActionCode _action;

		private EventReceiptAction(EventIdentificationTypeEventActionCode action)
		{
			_action = action;
		}

		public static implicit operator EventIdentificationTypeEventActionCode(EventReceiptAction operand)
		{
			return operand._action;
		}
	}
}