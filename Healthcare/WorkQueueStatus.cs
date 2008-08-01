using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// WorkQueueStatus enumeration
	/// </summary>
	[EnumValueClass(typeof(WorkQueueStatusEnum))]
	public enum WorkQueueStatus
	{
		[EnumValue("Pending")]
		PN,

		[EnumValue("Complete")]
		CM,

		[EnumValue("Failed")]
		F
	}
}