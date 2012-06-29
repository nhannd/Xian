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
using ClearCanvas.Common.Serialization;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class CompositeIdentifierDetail : DataContractBase, ICloneable
    {
        public CompositeIdentifierDetail(string id, EnumValueInfo assigningAuthority)
        {
            this.Id = id;
            this.AssigningAuthority = assigningAuthority;
        }

        public CompositeIdentifierDetail()
        {
        }

        [DataMember]
        public string Id;

        [DataMember]
        public EnumValueInfo AssigningAuthority;

        #region ICloneable Members

        public object Clone()
        {
            return new CompositeIdentifierDetail(this.Id, (EnumValueInfo)this.AssigningAuthority.Clone());
        }

        #endregion
    }
}
