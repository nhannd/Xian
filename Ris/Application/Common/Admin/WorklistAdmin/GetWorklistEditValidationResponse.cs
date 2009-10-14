using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin
{
	[DataContract]
	public class GetWorklistEditValidationResponse : DataContractBase
	{
		public GetWorklistEditValidationResponse()
		{
		}

		public GetWorklistEditValidationResponse(string errorMessage)
		{
			HasError = true;
			ErrorMessage = errorMessage;
		}

		[DataMember]
		public bool HasError;

		[DataMember]
		public string ErrorMessage;
	}
}
