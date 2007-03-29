using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin
{
    [DataContract]
    public class LoadNoteCategoryForEditRequest : DataContractBase
    {
        public LoadNoteCategoryForEditRequest(EntityRef NoteCategoryRef)
        {
            this.NoteCategoryRef = NoteCategoryRef;
        }

        [DataMember]
        public EntityRef NoteCategoryRef;
    }
}
