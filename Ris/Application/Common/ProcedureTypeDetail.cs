#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
    [DataContract]
    public class ProcedureTypeDetail : DataContractBase
    {
		public ProcedureTypeDetail()
		{
		}

		public ProcedureTypeDetail(
			EntityRef entityRef,
			string id,
			string name,
			ProcedureTypeSummary baseType,
			string planXml,
			int defaultDuration,
			bool deactivated)
        {
            this.ProcedureTypeRef = entityRef;
            this.Id = id;
            this.Name = name;
        	this.BaseType = baseType;
        	this.PlanXml = planXml;
			this.DefaultDuration = defaultDuration;
			this.Deactivated = deactivated;
        }

        [DataMember]
        public EntityRef ProcedureTypeRef;

        [DataMember]
        public string Id;

        [DataMember]
        public string Name;

		[DataMember]
		public ProcedureTypeSummary BaseType;

		[DataMember]
		public string PlanXml;

		[DataMember]
    	public int DefaultDuration;

		[DataMember]
		public bool Deactivated;

		public ProcedureTypeSummary GetSummary()
        {
            return new ProcedureTypeSummary(this.ProcedureTypeRef, this.Name, this.Id, this.DefaultDuration, this.Deactivated);
        }
    }
}
