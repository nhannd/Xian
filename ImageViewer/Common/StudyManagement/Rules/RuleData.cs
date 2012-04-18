using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
    [DataContract(Namespace = RulesNamespace.Value)]
	[StudyRuleDataContract("8894a4cb-17e0-4efc-b90c-2a8985d8f516")]
	public class RuleData
	{
		public RuleData()
		{
			this.Enabled = true;
			this.Conditions = new List<RuleCondition>();
			this.Actions = new List<RuleAction>();
		}

		[DataMember]
		public string Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool Enabled { get; set; }

		[DataMember]
		public JunctionType Junction { get; set; }

		[DataMember]
		public List<RuleCondition> Conditions { get; set; }

		[DataMember]
		public List<RuleAction> Actions { get; set; }
	}
}
