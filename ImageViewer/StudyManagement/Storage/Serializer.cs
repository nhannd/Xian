#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	internal class Serializer
	{
		private static readonly IJsmlSerializerHook _workItemRequestHook = new PolymorphicDataContractHook<WorkItemRequestDataContractAttribute>();
		private static readonly IJsmlSerializerHook _workItemProgressHook = new PolymorphicDataContractHook<WorkItemProgressDataContractAttribute>();
        private static readonly IJsmlSerializerHook _serverExtensionDataHook = new PolymorphicDataContractHook<ServerDataContractAttribute>();

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

		public static string SerializeWorkItemProgress(WorkItemProgress data)
		{
			return JsmlSerializer.Serialize(data, "data",
				new JsmlSerializer.SerializeOptions { Hook = _workItemProgressHook, DataContractTest = IsWorkItemProgressContract });
		}
		public static WorkItemProgress DeserializeWorkItemProgress(string data)
		{
			return JsmlSerializer.Deserialize<WorkItemProgress>(data,
				new JsmlSerializer.DeserializeOptions { Hook = _workItemProgressHook, DataContractTest = IsWorkItemProgressContract });
		}

		private static bool IsWorkItemProgressContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemProgressDataContractAttribute>(t);
		}

		private static bool IsWorkItemRequestContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemRequestDataContractAttribute>(t);
		}

        private static bool IsServerExtensionDataContract(Type t)
		{
			return AttributeUtils.HasAttribute<ServerDataContractAttribute>(t);
		}

        public static string SerializeServerExtensionData(Dictionary<string, object> serverExtensionData)
        {
            return JsmlSerializer.Serialize(serverExtensionData, "data",
                new JsmlSerializer.SerializeOptions { Hook = _serverExtensionDataHook, DataContractTest = IsServerExtensionDataContract });
        }

        public static Dictionary<string, object> DeserializeServerExtensionData(string data)
        {
            return JsmlSerializer.Deserialize<Dictionary<string, object>>(data,
                new JsmlSerializer.DeserializeOptions { Hook = _serverExtensionDataHook, DataContractTest = IsServerExtensionDataContract });
        }
	}
}
