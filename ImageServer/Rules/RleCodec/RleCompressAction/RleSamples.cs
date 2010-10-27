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
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Rules.RleCodec.RleCompressAction
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class RleSample : SampleRuleBase
	{
		public RleSample()
			: base("RleParameters",
			       "RLE Sample Compression",
			       ServerRuleTypeEnum.StudyCompress,
			       "Sample_RLE.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class RleSopSample : SampleRuleBase
	{
		public RleSopSample()
			: base("RleSopSample",
				   "RLE Sample SOP Compression",
				   ServerRuleTypeEnum.SopCompress,
				   "SampleRleSop.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
		}
	}
}