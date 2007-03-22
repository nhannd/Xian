using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class PersonNameDetail : DataContractBase
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
    }
}
