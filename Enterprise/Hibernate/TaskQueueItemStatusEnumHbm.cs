#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Hibernate
{
	/// <summary>
	/// NHibernate mapping class for <see cref="TaskQueueItemStatusEnumHbm"/>.
	/// This file is machine generated - changes will be lost.
	/// </summary>
	public class TaskQueueItemStatusEnumHbm : NHibernate.Type.EnumStringType
	{
		public TaskQueueItemStatusEnumHbm()
			: base(typeof(TaskQueueItemStatus))
		{
		}
	}
}
