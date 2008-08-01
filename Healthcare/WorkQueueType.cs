using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// WorkQueueType enumeration
	/// </summary>
	[EnumValueClass(typeof(WorkQueueTypeEnum))]
	public enum WorkQueueType
	{
		[EnumValue("Fax Report")]
		FXR
	}
}