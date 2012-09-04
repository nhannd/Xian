#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class DiagnosticServicePlanDetail : DataContractBase
	{
		public DiagnosticServicePlanDetail()
		{
			this.ProcedureTypes = new List<ProcedureTypeDetail>();
		}

		public DiagnosticServicePlanDetail(EntityRef diagnosticServiceRef, string id, string name, List<ProcedureTypeDetail> procedureTypes)
		{
			this.DiagnosticServiceRef = diagnosticServiceRef;
			this.Id = id;
			this.Name = name;
			this.ProcedureTypes = procedureTypes;
		}

		[DataMember]
		public EntityRef DiagnosticServiceRef;

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;

		[DataMember]
		public List<ProcedureTypeDetail> ProcedureTypes;
	}
}
