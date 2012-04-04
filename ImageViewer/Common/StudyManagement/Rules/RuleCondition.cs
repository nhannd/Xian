using System.Linq;
using System.Runtime.Serialization;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
	[DataContract]
	[StudyRuleDataContract("664fcf90-fa7d-483f-b212-b1fe73e8d704")]
	public class RuleCondition
	{
		public static readonly DicomTag[] TagChoices = {
			DicomTagDictionary.GetDicomTag("Modality"),
			DicomTagDictionary.GetDicomTag("PatientId"),
			DicomTagDictionary.GetDicomTag("StudyDescription"),
			DicomTagDictionary.GetDicomTag("SourceApplicationEntityTitle"),
			DicomTagDictionary.GetDicomTag("StationName"),
			DicomTagDictionary.GetDicomTag("InstitutionName"),
			DicomTagDictionary.GetDicomTag("ReferringPhysiciansName"),
			DicomTagDictionary.GetDicomTag("TransferSyntaxUid"),
			DicomTagDictionary.GetDicomTag("PhotometricInterpretation"),
			DicomTagDictionary.GetDicomTag("BitsAllocated"),
			DicomTagDictionary.GetDicomTag("BitsStored"),
			DicomTagDictionary.GetDicomTag("PatientsBirthDate"),
			DicomTagDictionary.GetDicomTag("StudyDate")
		};

		public RuleCondition()
		{
			Tag = TagChoices.First();
		}

		public DicomTag Tag
		{
			get { return DicomTagDictionary.GetDicomTag(this.TagName); }
			set { this.TagName = value.VariableName; }
		}

		[DataMember]
		public ComparisonOperator Operator { get; set; }

		[DataMember]
		public string Value { get; set; }

		[DataMember]
		public string TagName { get; set; }
	}
}
