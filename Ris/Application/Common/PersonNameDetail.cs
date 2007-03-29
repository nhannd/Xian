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

        #region ICloneable Members

        public object Clone()
        {
            PersonNameDetail clone = new PersonNameDetail();
            clone.FamilyName = this.FamilyName;
            clone.GivenName = this.GivenName;
            clone.MiddleName = this.MiddleName;
            clone.Prefix = this.Prefix;
            clone.Suffix = this.Suffix;
            clone.Degree = this.Degree;

            return clone;
        }

        #endregion
    }
}
