#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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