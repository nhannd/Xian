using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Workflow
{
	/// <summary>
	/// WorkQueueStatus enumeration
	/// </summary>
	[EnumValueClass(typeof(WorkQueueStatusEnum))]
	public enum WorkQueueStatus
	{
		/// <summary>
		/// Pending
		/// </summary>
		[EnumValue("Pending")]
		PN,

		/// <summary>
		/// Complete
		/// </summary>
		[EnumValue("Complete")]
		CM,

		/// <summary>
		/// Failed
		/// </summary>
		[EnumValue("Failed")]
		F
	}
}