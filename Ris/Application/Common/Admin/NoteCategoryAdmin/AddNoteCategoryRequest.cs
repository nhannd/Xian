using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class AddNoteCategoryRequest : DataContractBase
    {
        public AddNoteCategoryRequest(NoteCategoryDetail detail)
        {
            this.NoteCategoryDetail = detail;
        }

        [DataMember]
        public NoteCategoryDetail NoteCategoryDetail;
    }
}
