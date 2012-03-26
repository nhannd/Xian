using System.Runtime.Serialization;
namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
	[DataContract]
	[StudyRuleDataContract("fafdf01d-c4c7-4ab9-a1d3-2912c8526c79")]
	public abstract class RuleAction
	{
	}

	[DataContract]
	[StudyRuleDataContract("689f312b-f3c4-4497-9f52-62318d327921")]
	public class SendStudyAction : RuleAction
	{
		public SendStudyAction()
		{
			this.SendImmediately = true;
		}

		[DataMember]
		public string Destination { get; set; }

		[DataMember]
		public bool SendImmediately { get; set; }

		[DataMember]
		public int TimeWindowStart { get; set; }

		[DataMember]
		public int TimeWindowEnd { get; set; }

		[DataMember]
		public CompressionType CompressionType { get; set; }

		[DataMember]
		public int CompressionLevel { get; set; }
	}

	[DataContract]
	[StudyRuleDataContract("bd06d020-d41f-4c34-9598-460022e3fca5")]
	public class DeleteStudyAction : RuleAction
	{
		[DataMember]
		public TimeOrigin TimeOrigin { get; set; }

		[DataMember]
		public TimeUnit TimeUnit { get; set; }

		[DataMember]
		public int TimeValue { get; set; }
	}
}
