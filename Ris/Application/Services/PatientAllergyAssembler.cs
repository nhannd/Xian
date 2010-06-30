using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class PatientAllergyAssembler
	{
		public PatientAllergyDetail CreateAllergyDetail(Allergy allergy)
		{
			return new PatientAllergyDetail();
		}
	}
}
