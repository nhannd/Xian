using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	public class BiographyAllergiesViewComponent : DHtmlComponent
	{
		// Internal data contract used for jscript deserialization
		[DataContract]
		public class AllergyContext : DataContractBase
		{
			[DataMember]
			public List<PatientAllergyDetail> Allergies;
		}

		private readonly AllergyContext _context;

		public BiographyAllergiesViewComponent()
			: this(null)
		{
		}

		public BiographyAllergiesViewComponent(List<PatientAllergyDetail> allergies)
		{
			_context = new AllergyContext {Allergies = allergies};
		}

		public override void Start()
		{
			SetUrl(WebResourcesSettings.Default.BiographyAllergyDetailPageUrl);
			base.Start();
		}

		protected override DataContractBase GetHealthcareContext()
		{
			return _context;
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
	}
}
