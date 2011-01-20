#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegBaselineAction
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class JpegBaselineSamples : SampleRuleBase
	{
		public JpegBaselineSamples()
			: base("JpegBaseline",
			       "JPEG Baseline Time Rule",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpegBaseline.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class JpegBaselineSampleStudyDate : SampleRuleBase
	{
		public JpegBaselineSampleStudyDate()
			: base("JpegBaselineStudyDate",
			       "JPEG Baseline, Study Date Time",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpegBaselineStudyDate.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class JpegBaselineSopSample : SampleRuleBase
	{
		public JpegBaselineSopSample()
			: base("JpegBaselineSopRfXaUs",
				   "JPEG Baseline SOP Compression, RF XA US Compress",
				   ServerRuleTypeEnum.SopCompress,
				   "SampleJpegBaselineSop.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
		}
	}
}