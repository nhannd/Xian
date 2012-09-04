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
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolCodeDetail : DataContractBase, IEquatable<ProtocolCodeDetail>
    {
		public ProtocolCodeDetail()
		{

		}

        public ProtocolCodeDetail(EntityRef entityRef, string name, string description, bool deactivated)
        {
            ProtocolCodeRef = entityRef;
            Name = name;
            Description = description;
        	Deactivated = deactivated;
        }

        [DataMember]
        public EntityRef ProtocolCodeRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

		[DataMember]
		public bool Deactivated;

        public bool Equals(ProtocolCodeDetail protocolCodeDetail)
        {
            if (protocolCodeDetail == null) return false;
            return Equals(this.ProtocolCodeRef, protocolCodeDetail.ProtocolCodeRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ProtocolCodeDetail);
        }

        public override int GetHashCode()
        {
			return ProtocolCodeRef.GetHashCode();
        }
    }
}