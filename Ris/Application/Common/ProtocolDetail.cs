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
	public class ProtocolDetail : DataContractBase
	{
		public ProtocolDetail()
		{
			Codes = new List<ProtocolCodeSummary>();
			Procedures = new List<ProcedureDetail>();
		}

		[DataMember]
		public EntityRef ProtocolRef;

		[DataMember]
		public StaffSummary Author;

		[DataMember]
		public StaffSummary Supervisor;

		[DataMember]
		public EnumValueInfo Status;

		[DataMember]
		public EnumValueInfo Urgency;

		[DataMember]
		public EnumValueInfo RejectReason;

		[DataMember]
		public List<ProtocolCodeSummary> Codes;

		[DataMember]
		public List<ProcedureDetail> Procedures;
	}
}