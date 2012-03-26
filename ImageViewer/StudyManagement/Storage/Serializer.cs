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
using ClearCanvas.ImageViewer.Common.StudyManagement.Rules;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
	internal class Serializer
	{
		private static readonly IJsmlSerializerHook _workItemRequestHook = new PolymorphicDataContractHook<WorkItemRequestDataContractAttribute>();
		private static readonly IJsmlSerializerHook _workItemProgressHook = new PolymorphicDataContractHook<WorkItemProgressDataContractAttribute>();
		private static readonly IJsmlSerializerHook _ruleHook = new PolymorphicDataContractHook<StudyRuleDataContractAttribute>();

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

		public static string SerializeRule(RuleData data)
		{
			return JsmlSerializer.Serialize(data, "data",
				new JsmlSerializer.SerializeOptions { Hook = _ruleHook, DataContractTest = IsStudyRuleContract });
		}

		public static RuleData DeserializeRule(string data)
		{
			return JsmlSerializer.Deserialize<RuleData>(data,
				new JsmlSerializer.DeserializeOptions { Hook = _ruleHook, DataContractTest = IsStudyRuleContract });
		}

		private static bool IsWorkItemProgressContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemProgressDataContractAttribute>(t);
		}

		private static bool IsWorkItemRequestContract(Type t)
		{
			return AttributeUtils.HasAttribute<WorkItemRequestDataContractAttribute>(t);
		}

		private static bool IsStudyRuleContract(Type t)
		{
			return AttributeUtils.HasAttribute<StudyRuleDataContractAttribute>(t);
		}
	}
}
