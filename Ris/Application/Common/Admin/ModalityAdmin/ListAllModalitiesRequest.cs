using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ModalityAdmin
{
    [DataContract]
    public class ListAllModalitiesRequest : DataContractBase
    {
        public ListAllModalitiesRequest(bool activeOnly)
        {
            this.ActiveOnly = activeOnly; 
        }

        [DataMember]
        public bool ActiveOnly;

    }
}
