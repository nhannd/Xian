#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.ServiceModel.Query;
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

		public StudyIdentifier StudyData
		{
			get
			{
				return Serializer.DeserializeStudyData(this.SerializedStudy);
			}
			set
			{
				this.SerializedStudy = Serializer.SerializeStudyData(value);
			}
		}

	}
}
