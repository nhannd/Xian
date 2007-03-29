using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class GetNoteCategoryEditFormDataResponse : DataContractBase
    {
        public GetNoteCategoryEditFormDataResponse(List<EnumValueInfo> severityChoices)
        {
            this.SeverityChoices = severityChoices;
        }

        [DataMember]
        public List<EnumValueInfo> SeverityChoices;
    }


}
