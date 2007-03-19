using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class TelephoneDetail : DataContractBase, ICloneable
    {
        [DataMember]
        public string CountryCode;

        [DataMember]
        public string AreaCode;

        [DataMember]
        public string Number;

        [DataMember]
        public string Extension;

        [DataMember]
        public EnumValueInfo Type;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;

        #region ICloneable Members

        public object Clone()
        {
            TelephoneDetail clone = new TelephoneDetail();
            clone.AreaCode = this.AreaCode;
            clone.CountryCode = this.CountryCode;
            clone.Extension = this.Extension;
            clone.Number = this.Number;
            clone.Type = (EnumValueInfo)this.Type.Clone();
            clone.ValidRangeFrom = this.ValidRangeFrom;
            clone.ValidRangeUntil = this.ValidRangeUntil;

            return clone;
        }

        #endregion
    }
}
