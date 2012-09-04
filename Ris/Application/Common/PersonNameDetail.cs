#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

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
