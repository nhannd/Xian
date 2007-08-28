using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class CompositeIdentifierDetail : DataContractBase, ICloneable
    {
        public CompositeIdentifierDetail(string id, string assigningAuthority)
        {
            this.Id = id;
            this.AssigningAuthority = assigningAuthority;
        }

        public CompositeIdentifierDetail()
        {
        }

        [DataMember]
        public string Id;

        [DataMember]
        public string AssigningAuthority;

        #region ICloneable Members

        public object Clone()
        {
            return new CompositeIdentifierDetail(this.Id, this.AssigningAuthority);
        }

        #endregion
    }
}
