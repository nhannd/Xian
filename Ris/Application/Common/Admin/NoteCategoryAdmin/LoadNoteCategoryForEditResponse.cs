using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class LoadNoteCategoryForEditResponse : DataContractBase
    {
        public LoadNoteCategoryForEditResponse(EntityRef NoteCategoryRef, NoteCategoryDetail NoteCategoryDetail)
        {
            this.NoteCategoryRef = NoteCategoryRef;
            this.NoteCategoryDetail = NoteCategoryDetail;
        }


        [DataMember]
        public EntityRef NoteCategoryRef;

        [DataMember]
        public NoteCategoryDetail NoteCategoryDetail;
    }
}
