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
            : this(code, value, "")
        {
        }

        public EnumValueInfo(string code, string value, string description)
        {
            this.Code = code;
            this.Value = value;
            this.Description = description;
        }

        public EnumValueInfo()
        {
        }

        [DataMember]
        public string Code;

        [DataMember]
        public string Value;

        [DataMember]
        public string Description;

        #region ICloneable Members

        public object Clone()
        {
            return new EnumValueInfo(this.Code, this.Value, this.Description);
        }

        #endregion
    }
}
