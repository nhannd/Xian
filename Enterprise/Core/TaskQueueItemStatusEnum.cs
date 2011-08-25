#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core.Modelling;

namespace ClearCanvas.Enterprise.Core
{
	/// <summary>
	/// TaskQueueItemStatusEnum enumeration
	/// </summary>
	[UniqueKey("Code", new[] { "Code" })]
	[UniqueKey("Value", new[] { "Value" })]
	public class TaskQueueItemStatusEnum : EnumValue
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		protected TaskQueueItemStatusEnum()
		{
		}

		/// <summary>
		/// Constructor for creating dummy values during unit testing. Not for production use.
		/// </summary>
		public TaskQueueItemStatusEnum(string code, string value, string description)
			: base(code, value, description)
		{
		}
	}
}
