using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class GetOperationEnablementResponse : DataContractBase
	{
		public GetOperationEnablementResponse(Dictionary<string, bool> dictionary)
		{
			this.OperationEnablementDictionary = dictionary;
		}

		[DataMember]
		public Dictionary<string, bool> OperationEnablementDictionary;
	}
}
