#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Common.WorkItem;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	partial class WorkItem
	{
		public WorkItemRequest Request
		{
			get
			{
				return Serializer.DeserializeWorkItemRequest(this.SerializedRequest);
			}
			set
			{
				this.SerializedRequest = Serializer.SerializeWorkItemRequest(value);
			}
		}

		public WorkItemProgress Progress
		{
			get
			{
				return Serializer.DeserializeWorkItemProgress(this.SerializedProgress);
			}
			set
			{
                this.SerializedProgress = Serializer.SerializeWorkItemProgress(value);
			}
		}
	}
}
