#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Enterprise.Core
{
	[EnumValueClass(typeof(TaskQueueItemStatusEnum))]
	public enum TaskQueueItemStatus
	{
		/// <summary>
		/// Scheduled
		/// </summary>
		[EnumValue("Scheduled", Description = "Scheduled")]
		Scheduled,

		/// <summary>
		/// Completed
		/// </summary>
		[EnumValue("Completed", Description = "Completed")]
		Completed,

		/// <summary>
		/// In Process
		/// </summary>
		[EnumValue("In Process", Description = "In process")]
		InProcess,

		/// <summary>
		/// Failed
		/// </summary>
		[EnumValue("Failed", Description = "Failed")]
		Failed,
	}
}
