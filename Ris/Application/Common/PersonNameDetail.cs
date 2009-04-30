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
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PersonNameDetail : DataContractBase, ICloneable
    {
        public PersonNameDetail(string familyName, string givenName, string middleName, string prefix, string suffix, string degree)
        {
            this.FamilyName = familyName;
            this.GivenName = givenName;
            this.MiddleName = middleName;
            this.Prefix = prefix;
            this.Suffix = suffix;
            this.Degree = degree;
        }

        public PersonNameDetail()
        {
        }

        [DataMember]
        public string FamilyName;

        [DataMember]
        public string GivenName;

        [DataMember]
        public string MiddleName;

        [DataMember]
        public string Prefix;

        [DataMember]
        public string Suffix;

        [DataMember]
        public string Degree;

        public override string ToString()
        {
            return string.Format("{0}, {1}", this.FamilyName, this.GivenName);
        }

        #region ICloneable Members

        public object Clone()
        {
        	return new PersonNameDetail(this.FamilyName, this.GivenName, this.MiddleName,
				this.Prefix, this.Suffix, this.Degree);
        }

        #endregion
    }
}
