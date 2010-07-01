using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class PatientAllergiesComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class AllergyContext : DataContractBase
		{
			public AllergyContext()
			{
				this.Allergies = new List<PatientAllergyDetail>();
				this.AllergenTypeChoices = new List<EnumValueInfo>();
				this.SeverityChoices = new List<EnumValueInfo>();
				this.SensitivityTypeChoices = new List<EnumValueInfo>();
				this.ReportedByRelationshipTypeChoices = new List<EnumValueInfo>();
			}

			[DataMember]
			public List<PatientAllergyDetail> Allergies;

			[DataMember]
			public List<EnumValueInfo> AllergenTypeChoices;

			[DataMember]
			public List<EnumValueInfo> SeverityChoices;

			[DataMember]
			public List<EnumValueInfo> SensitivityTypeChoices;

			[DataMember]
			public List<EnumValueInfo> ReportedByRelationshipTypeChoices;
		}

		private readonly bool _readOnly;
		private readonly AllergyContext _context;

		/// <summary>
		/// Constructor for readonly allergies information.
		/// </summary>
		public PatientAllergiesComponent()
		{
			_readOnly = true;
			_context = new AllergyContext();
		}

		/// <summary>
		/// Constructor for editing allergies.
		/// </summary>
		/// <param name="allergenTypeChoices"></param>
		/// <param name="severityChoices"></param>
		/// <param name="sensitivityTypeChoices"></param>
		/// <param name="reportedByRelationshipTypeChoices"></param>
		public PatientAllergiesComponent(
			List<EnumValueInfo> allergenTypeChoices,
			List<EnumValueInfo> severityChoices,
			List<EnumValueInfo> sensitivityTypeChoices,
			List<EnumValueInfo> reportedByRelationshipTypeChoices)
		{
			_readOnly = false;
			_context = new AllergyContext
				{
					AllergenTypeChoices = allergenTypeChoices,
					SeverityChoices = severityChoices,
					SensitivityTypeChoices = sensitivityTypeChoices,
					ReportedByRelationshipTypeChoices = reportedByRelationshipTypeChoices
				};
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.BiographyAllergyDetailPageUrl);
			base.Start();
		}

		public List<PatientAllergyDetail> Allergies
		{
			get { return _context.Allergies; }
			set
			{
				_context.Allergies = value;
				NotifyAllPropertiesChanged();
			}
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
		}

		protected override string GetTag(string tag)
		{
			if (string.Equals("ReadOnly", tag))
			{
				return _readOnly ? "true" : "false";
			}

			return base.GetTag(tag);
		}
	}
}
