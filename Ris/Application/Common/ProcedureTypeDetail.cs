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
			ModalitySummary defaultModality,
			int defaultDuration,
			bool deactivated)
        {
            this.ProcedureTypeRef = entityRef;
            this.Id = id;
            this.Name = name;
			this.CustomProcedurePlan = false;
			this.DefaultModality = defaultModality;
			this.DefaultDuration = defaultDuration;
			this.Deactivated = deactivated;
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
			this.CustomProcedurePlan = true;
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
		public int DefaultDuration;

		[DataMember]
		public bool Deactivated;

		/// <summary>
		/// Specifies the default modality used by the default procedure plan (assuming <see cref="CustomProcedurePlan"/> is false).
		/// </summary>
		[DataMember]
    	public ModalitySummary DefaultModality;

		/// <summary>
		/// Specifies whether a custom procedure plan is used.
		/// </summary>
		[DataMember]
    	public bool CustomProcedurePlan;

		/// <summary>
		/// Specifies the base type, or null if <see cref="CustomProcedurePlan"/> is false.
		/// </summary>
		[DataMember]
		public ProcedureTypeSummary BaseType;

		/// <summary>
		/// Specifies the custom plan XML, or null if <see cref="CustomProcedurePlan"/> is false.
		/// </summary>
		[DataMember]
		public string PlanXml;


		public ProcedureTypeSummary GetSummary()
        {
            return new ProcedureTypeSummary(this.ProcedureTypeRef, this.Name, this.Id, this.DefaultDuration, this.Deactivated);
        }
    }
}
