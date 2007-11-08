using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProtocolCodeDetail : DataContractBase, IEquatable<ProtocolCodeDetail>
    {
        public ProtocolCodeDetail(EntityRef entityRef, string name, string description)
        {
            EntityRef = entityRef;
            Name = name;
            Description = description;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;


        public bool Equals(ProtocolCodeDetail protocolCodeDetail)
        {
            if (protocolCodeDetail == null) return false;
            return Equals(Name, protocolCodeDetail.Name) && Equals(Description, protocolCodeDetail.Description);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ProtocolCodeDetail);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + 29*(Description != null ? Description.GetHashCode() : 0);
        }
    }
}