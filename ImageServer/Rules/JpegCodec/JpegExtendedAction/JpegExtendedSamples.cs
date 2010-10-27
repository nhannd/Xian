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

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegExtendedAction
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class JpegExtendedSamples : SampleRuleBase
	{
		public JpegExtendedSamples()
			: base("JpegExtendedSample",
			       "JPEG Extended Simple Sample",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpegExtended.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class JpegExtendedSopSample : SampleRuleBase
	{
		public JpegExtendedSopSample()
			: base("JpegExtendedSopSample",
				   "JPEG Extended SOP Simple Sample",
				   ServerRuleTypeEnum.SopCompress,
				   "SampleJpegExtendedSop.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
		}
	}
}