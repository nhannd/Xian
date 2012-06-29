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
    public class EmailAddressDetail : DataContractBase, ICloneable
    {
        public EmailAddressDetail(string address, DateTime? validRangeFrom, DateTime? validRangeUntil)
        {
            this.Address = address;
            this.ValidRangeFrom = validRangeFrom;
            this.ValidRangeUntil = validRangeUntil;
        }

        public EmailAddressDetail()
        {
        }
    
        [DataMember]
        public string Address;

        [DataMember]
        public DateTime? ValidRangeFrom;

        [DataMember]
        public DateTime? ValidRangeUntil;

        #region ICloneable Members

        public object Clone()
        {
            EmailAddressDetail clone = new EmailAddressDetail();
            clone.Address = this.Address;
            clone.ValidRangeFrom = this.ValidRangeFrom;
            clone.ValidRangeUntil = this.ValidRangeUntil;
            return clone;
        }

        #endregion
    }
}
