#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common.Serialization;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	internal class Serializer
	{
		private static readonly IJsmlSerializerHook _workItemRequestHook = new PolymorphicDataContractHook<WorkItemRequestDataContractAttribute>();

		public static string SerializeWorkItemRequest(WorkItemRequest data)
		{
			return JsmlSerializer.Serialize(data, "data",
				new JsmlSerializer.SerializeOptions { Hook = _workItemRequestHook, DataContractTest = IsWorkItemRequestContract });
		}

		public static WorkItemRequest DeserializeWorkItemRequest(string data)
		{
			return JsmlSerializer.Deserialize<WorkItemRequest>(data,
				new JsmlSerializer.DeserializeOptions { Hook = _workItemRequestHook, DataContractTest = IsWorkItemRequestContract });
		}

		public static string SerializeStudyData(StudyIdentifier data)
		{
			return JsmlSerializer.Serialize(data, "data");
		}

		public static StudyIdentifier DeserializeStudyData(string data)
		{
			return JsmlSerializer.Deserialize<StudyIdentifier>(data);
		}

		private static bool IsWorkItemRequestContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemRequestDataContractAttribute>(t);
		}
	}
}
