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

namespace ClearCanvas.ImageServer.Rules.Jpeg2000Codec.Jpeg2000LosslessAction
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class Jpeg2000LosslessSamples : SampleRuleBase
	{
		public Jpeg2000LosslessSamples()
			: base("Jpeg2000Lossless",
			       "JPEG 2000 Lossless Sample Rule",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpeg2000Lossless.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

	[ExtensionOf(typeof(SampleRuleExtensionPoint))]
	public class Jpeg2000ComboSample : SampleRuleBase
	{
		public Jpeg2000ComboSample()
			: base("Jpeg2000Combo",
			       "JPEG 2000 Lossless and Lossy Sample Rule",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpeg2000Combo.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}
}