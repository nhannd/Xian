using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.Admin;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [DataContract]
    public class ListPatientOrdersResponse : DataContractBase
    {
        public ListPatientOrdersResponse(List<PatientOrderData> listData)
        {
            this.PatientOrderData = listData;
        }

        public ListPatientOrdersResponse()
        {
        }

        [DataMember]
        public List<PatientOrderData> PatientOrderData;
    }
}
