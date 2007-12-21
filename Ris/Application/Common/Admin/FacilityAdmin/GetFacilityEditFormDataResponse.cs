using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin
{
    [DataContract]
    public class GetFacilityEditFormDataResponse : DataContractBase
    {
        public GetFacilityEditFormDataResponse(List<EnumValueInfo> informationAuthorityChoices)
        {
            this.InformationAuthorityChoices = informationAuthorityChoices;
        }

        [DataMember]
        public List<EnumValueInfo> InformationAuthorityChoices;
    }
}
