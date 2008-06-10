using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.BrowsePatientData
{
	[DataContract]
	public class GetVisitDetailRequest : DataContractBase
	{
        public GetVisitDetailRequest(EntityRef visitRef)
        {
            this.VisitRef = visitRef;
        }

		public GetVisitDetailRequest()
        {
        }

        [DataMember]
        public EntityRef VisitRef;
	}
}
