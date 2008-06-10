using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class ListVisitsResponse : DataContractBase
	{
		public ListVisitsResponse(List<VisitListItem> listData)
        {
			this.Visits = listData;
        }

		public ListVisitsResponse()
        {
        }

        [DataMember]
        public List<VisitListItem> Visits;
	}
}
