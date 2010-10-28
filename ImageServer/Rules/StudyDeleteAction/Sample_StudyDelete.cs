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

namespace ClearCanvas.ImageServer.Rules.StudyDeleteAction
{
 	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class AgeBasedDeleteSample : SampleRuleBase
	{
		public AgeBasedDeleteSample()
			: base("AgeBasedDelete",
				   "Age Based Delete",
				   ServerRuleTypeEnum.StudyDelete,
				   "Sample_StudyDeleteAgeBased.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class MultiTagDeleteSample : SampleRuleBase
	{
		public MultiTagDeleteSample()
			: base("TagBasedDelete",
				   "Tag Based Delete",
				   ServerRuleTypeEnum.StudyDelete,
				   "Sample_StudyDeleteTagBased.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
		}
	}
}