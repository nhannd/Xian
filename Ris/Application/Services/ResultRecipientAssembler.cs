using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class ResultRecipientAssembler
	{
		public ResultRecipientDetail CreateResultRecipientDetail(ResultRecipient r, IPersistenceContext context)
		{
			ExternalPractitionerAssembler pracAssembler = new ExternalPractitionerAssembler();

			return new ResultRecipientDetail(
				pracAssembler.CreateExternalPractitionerSummary(r.PractitionerContactPoint.Practitioner, context),
				pracAssembler.CreateExternalPractitionerContactPointDetail(r.PractitionerContactPoint, context),
				EnumUtils.GetEnumValueInfo(r.PreferredCommunicationMode, context));
		}
	}
}