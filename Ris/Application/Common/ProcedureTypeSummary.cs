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
    public class ProcedureTypeSummary : DataContractBase, IEquatable<ProcedureTypeSummary>
    {
        public ProcedureTypeSummary(EntityRef entityRef, string name, string id, bool deactivated)
        {
            this.ProcedureTypeRef = entityRef;
            this.Name = name;
            this.Id = id;
        	this.Deactivated = deactivated;
        }

		public ProcedureTypeSummary()
		{
		}

        [DataMember]
        public EntityRef ProcedureTypeRef;

        [DataMember]
        public string Name;

        [DataMember]
        public string Id;

		[DataMember]
		public bool Deactivated;

		public bool Equals(ProcedureTypeSummary that)
        {
            if (that == null) return false;
            return Equals(this.ProcedureTypeRef, that.ProcedureTypeRef);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ProcedureTypeSummary);
        }

        public override int GetHashCode()
        {
            return ProcedureTypeRef.GetHashCode();
        }
    }
}