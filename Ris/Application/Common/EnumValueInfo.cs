#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

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
            if (ReferenceEquals(this, enumValueInfo)) return true;
            if (enumValueInfo == null) return false;
            if (!Equals(Code, enumValueInfo.Code)) return false;
            return true;
        }

        #endregion

        #region Object overrides

        public override bool Equals(object obj)
        {
            return Equals(obj as EnumValueInfo);
        }

        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }

        /// <summary>
        /// Return the display value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Value;
        }

        #endregion
    }
}
