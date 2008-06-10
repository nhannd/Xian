using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class ListWorklistsForUserResponse : DataContractBase
	{
		public ListWorklistsForUserResponse(List<WorklistSummary> worklists)
        {
            Worklists = worklists;
        }

        [DataMember]
        public List<WorklistSummary> Worklists;
	}
}
