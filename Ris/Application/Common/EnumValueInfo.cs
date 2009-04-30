#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
