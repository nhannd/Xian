using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
	[DataContract]
	public class LoadModalityEditorFormDataResponse : DataContractBase
	{
		[DataMember]
		public List<EnumValueInfo> DicomModalityChoices;
	}
}
