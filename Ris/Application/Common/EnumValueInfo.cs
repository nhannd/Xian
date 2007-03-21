using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class EnumValueInfo : DataContractBase, ICloneable
    {
        public EnumValueInfo(string code, string value)
        {
            this.Code = code;
            this.Value = value;
        }

        public EnumValueInfo()
        {
        }

        [DataMember]
        public string Code;

        [DataMember]
        public string Value;

        #region ICloneable Members

        public object Clone()
        {
            EnumValueInfo clone = new EnumValueInfo();
            clone.Code = this.Code;
            clone.Value = this.Value;

            return clone;
        }

        #endregion
    }
}
