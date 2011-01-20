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

namespace ClearCanvas.ImageServer.Rules.Jpeg2000Codec.Jpeg2000LossyAction
{
	[ExtensionOf(typeof (SampleRuleExtensionPoint))]
	public class Jpeg2000LossySamples : SampleRuleBase
	{
		public Jpeg2000LossySamples()
			: base("Jpeg2000Lossy",
			       "JPEG 2000 Lossy Sample Rule",
			       ServerRuleTypeEnum.StudyCompress,
			       "SampleJpeg2000Lossy.xml")
		{
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyProcessed);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyArchived);
			ApplyTimeList.Add(ServerRuleApplyTimeEnum.StudyRestored);
		}
	}
}