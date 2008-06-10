using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class GetVisitDetailResponse : DataContractBase
	{
        public GetVisitDetailResponse(VisitDetail visitDetail)
        {
			this.Visit = visitDetail;
        }

		public GetVisitDetailResponse()
        {
        }

        [DataMember]
		public VisitDetail Visit;

	}
}
