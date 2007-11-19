using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Login
{
    [DataContract]
    public class GetWorkingFacilityChoicesResponse : DataContractBase
    {
        public GetWorkingFacilityChoicesResponse(List<FacilitySummary> workingFacilityChoices)
        {
            this.FacilityChoices = workingFacilityChoices;
        }

        /// <summary>
        /// List of facilities that the user may choose an working facility.
        /// </summary>
        [DataMember]
        public List<FacilitySummary> FacilityChoices;
    }
}
