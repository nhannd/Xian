using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class LoadCannedTextForEditResponse : DataContractBase
	{
		public LoadCannedTextForEditResponse(CannedTextDetail detail)
		{
			this.CannedTextDetail = detail;
		}

		[DataMember]
		public CannedTextDetail CannedTextDetail;
	}
}
