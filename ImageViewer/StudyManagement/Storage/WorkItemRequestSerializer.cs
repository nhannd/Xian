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
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	class WorkItemRequestSerializer
	{
		private static readonly IJsmlSerializerHook _hook = new PolymorphicDataContractHook<WorkItemRequestDataContractAttribute>();

		public static string Serialize(WorkItemRequest data)
		{
			return JsmlSerializer.Serialize(data, "data",
				new JsmlSerializer.SerializeOptions { Hook = _hook, DataContractTest = IsDataContract });
		}

		public static WorkItemRequest Deserialize(string data)
		{
			return JsmlSerializer.Deserialize<WorkItemRequest>(data,
				new JsmlSerializer.DeserializeOptions { Hook = _hook, DataContractTest = IsDataContract });
		}

		private static bool IsDataContract(Type t)
		{
			return AttributeUtils.HasAttribute<DataContractAttribute>(t) ||
				   AttributeUtils.HasAttribute<WorkItemRequestDataContractAttribute>(t);
		}
	}
}
