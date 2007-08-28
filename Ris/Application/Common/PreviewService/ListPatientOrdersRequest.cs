using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.PreviewService
{
    [Serializable]
    public enum PatientOrdersQueryDetailLevel
    {
        Order,
        RequestedProcedure,
        ModalityProcedureStep
    }

    [DataContract]
    public class ListPatientOrdersRequest : DataContractBase
    {
        public ListPatientOrdersRequest(PatientOrdersQueryDetailLevel queryDetailLevel)
        {
            this.QueryDetailLevel = queryDetailLevel;
        }

        public ListPatientOrdersRequest()
        {
        }

        [DataMember]
        public PatientOrdersQueryDetailLevel QueryDetailLevel;
    }
}
