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
