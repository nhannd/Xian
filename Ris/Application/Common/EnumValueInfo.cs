using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class EnumValueInfo : DataContractBase, ICloneable, IEquatable<EnumValueInfo>
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

        #region IEquatable<EnumValueInfo>

        public bool Equals(EnumValueInfo enumValueInfo)
        {
            if (enumValueInfo == null) return false;
            if (!Equals(Code, enumValueInfo.Code)) return false;
            if (!Equals(Value, enumValueInfo.Value)) return false;
            if (!Equals(Description, enumValueInfo.Description)) return false;
            return true;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as EnumValueInfo);
        }

        public override int GetHashCode()
        {
            int result = Code.GetHashCode();
            result = 29*result + Value.GetHashCode();
            result = 29*result + (Description != null ? Description.GetHashCode() : 0);
            return result;
        }

        #endregion
    }
}
