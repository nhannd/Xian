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

namespace ClearCanvas.ImageServer.Rules.JpegCodec.JpegLosslessAction
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class JpegLosslessSamples : SampleRuleBase
	{
		public JpegLosslessSamples()
			: base("JpegLossless",
			       "JPEG Lossless Sample Rule",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpegLossless.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class JpegLosslessSopSample : SampleRuleBase
	{
		public JpegLosslessSopSample()
			: base("JpegLosslessSop",
				   "JPEG Lossless SOP Sample Rule",
				   ServerRuleTypeEnum.SopCompress,
				   "SampleJpegLosslessSop.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.SopProcessed);
		}
	}
}