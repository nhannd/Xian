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

namespace ClearCanvas.ImageServer.Rules.CompressSamples
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class LosslessCompressExemptSample : SampleRuleBase
	{
		public LosslessCompressExemptSample()
			: base("CompressExempt",
			       "Compress Exempt Rule",
				   ServerRuleTypeEnum.StudyCompress,
			       "SampleCompressExempt.xml")
		{
		    ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}

}