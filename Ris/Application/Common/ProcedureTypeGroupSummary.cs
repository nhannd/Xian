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
    public class ProcedureTypeGroupSummary : DataContractBase, IEquatable<ProcedureTypeGroupSummary>
    {
        public ProcedureTypeGroupSummary(EntityRef entityRef, string name, string description, EnumValueInfo category)
        {
			ProcedureTypeGroupRef = entityRef;
            Name = name;
            Description = description;
            Category = category;
        }

        [DataMember]
        public EntityRef ProcedureTypeGroupRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Description;

        [DataMember]
        public EnumValueInfo Category;


        public bool Equals(ProcedureTypeGroupSummary procedureTypeGroupSummary)
        {
            if (procedureTypeGroupSummary == null) return false;
            return Equals(this.ProcedureTypeGroupRef, procedureTypeGroupSummary.ProcedureTypeGroupRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ProcedureTypeGroupSummary);
        }

        public override int GetHashCode()
        {
        	return ProcedureTypeGroupRef.GetHashCode();
        }
    }
}