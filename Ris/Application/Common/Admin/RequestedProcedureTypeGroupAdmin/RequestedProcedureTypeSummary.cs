using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin.RequestedProcedureTypeGroupAdmin
{
    [DataContract]
    public class RequestedProcedureTypeSummary : DataContractBase, IEquatable<RequestedProcedureTypeSummary>
    {
        public RequestedProcedureTypeSummary(EntityRef entityRef, string name, string id)
        {
            this.EntityRef = entityRef;
            this.Name = name;
            this.Id = id;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Id;

        public bool Equals(RequestedProcedureTypeSummary requestedProcedureTypeSummary)
        {
            if (requestedProcedureTypeSummary == null) return false;
            return Equals(Id, requestedProcedureTypeSummary.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as RequestedProcedureTypeSummary);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}