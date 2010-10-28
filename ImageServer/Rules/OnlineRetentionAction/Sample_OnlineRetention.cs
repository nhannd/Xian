#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.OnlineRetentionAction
{
	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class SimpleOnlineRetention : SampleRuleBase
	{
		public SimpleOnlineRetention()
			: base("SimpleOnlineRetention",
				   "Simple Online Retention",
				   ServerRuleTypeEnum.OnlineRetention,
				   "Sample_OnlineRetentionSimple.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}
}
