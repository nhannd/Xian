#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Rules.Tier1RetentionAction
{
	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class StudyDateTier1Retention : SampleRuleBase
	{
		public StudyDateTier1Retention()
			: base("StudyDateTier1Retention",
				   "Study Date Based Tier1 Retention",
				   ServerRuleTypeEnum.Tier1Retention,
				   "Sample_StudyDateTier1Retention.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
		}
	}
}
