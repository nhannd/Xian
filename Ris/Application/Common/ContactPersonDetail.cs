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
    public class ContactPersonDetail : DataContractBase, ICloneable
    {
        public ContactPersonDetail(EnumValueInfo type, string name, string address, string homePhoneNumber, string businessPhoneNumber, EnumValueInfo relationship)
        {
            this.Type = type;
            this.Name = name;
            this.Address = address;
            this.HomePhoneNumber = homePhoneNumber;
            this.BusinessPhoneNumber = businessPhoneNumber;
            this.Relationship = relationship;
        }

        public ContactPersonDetail()
        {
        }
        
        [DataMember]
        public EnumValueInfo Type;

        [DataMember]
        public string Name;

        [DataMember]
        public string Address;

        [DataMember]
        public string HomePhoneNumber;

        [DataMember]
        public string BusinessPhoneNumber;

        [DataMember]
        public EnumValueInfo Relationship;

        #region ICloneable Members

        public object Clone()
        {
            ContactPersonDetail clone = new ContactPersonDetail();
            clone.Address = this.Address;
            clone.BusinessPhoneNumber = this.BusinessPhoneNumber;
            clone.HomePhoneNumber = this.HomePhoneNumber;
            clone.Name = this.Name;
            clone.Relationship = (EnumValueInfo)this.Relationship.Clone();
            clone.Type = (EnumValueInfo)this.Type.Clone();

            return clone;
        }

        #endregion
    }
}
