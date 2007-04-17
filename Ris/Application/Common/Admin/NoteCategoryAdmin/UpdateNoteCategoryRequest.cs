using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class UpdateNoteCategoryRequest : DataContractBase
    {
        public UpdateNoteCategoryRequest(EntityRef NoteCategoryRef, NoteCategoryDetail detail)
        {
            this.NoteCategoryRef = NoteCategoryRef;
            this.NoteCategoryDetail = detail;
        }

        [DataMember]
        public EntityRef NoteCategoryRef;

        [DataMember]
        public NoteCategoryDetail NoteCategoryDetail;
    }
}
