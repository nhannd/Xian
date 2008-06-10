using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ListWorklistsForUserRequest : DataContractBase
	{
		public ListWorklistsForUserRequest(List<string> worklistTokens)
        {
            this.WorklistTokens = worklistTokens;
        }

        [DataMember]
        public List<string> WorklistTokens;
	}
}
