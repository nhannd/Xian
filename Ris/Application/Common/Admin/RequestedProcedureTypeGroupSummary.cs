using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.Admin
{
    [DataContract]
    public class RequestedProcedureTypeGroupSummary : DataContractBase, IEquatable<RequestedProcedureTypeGroupSummary>
    {
        public RequestedProcedureTypeGroupSummary(EntityRef entityRef, string name, string description, EnumValueInfo category)
        {
            EntityRef = entityRef;
            Name = name;
            Description = description;
            Category = category;
        }

        [DataMember]
        public EntityRef EntityRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Category;


        public bool Equals(RequestedProcedureTypeGroupSummary requestedProcedureTypeGroupSummary)
        {
            if (requestedProcedureTypeGroupSummary == null) return false;
            return Equals(Name, requestedProcedureTypeGroupSummary.Name) && Equals(Category, requestedProcedureTypeGroupSummary.Category);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as RequestedProcedureTypeGroupSummary);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + 29*Category.GetHashCode();
        }
    }
}