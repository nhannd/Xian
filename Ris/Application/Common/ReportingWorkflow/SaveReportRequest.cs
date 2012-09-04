#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SaveReportRequest : DataContractBase
	{
		public SaveReportRequest(EntityRef reportingStepRef, Dictionary<string, string> reportPartExtendedProperties, EntityRef supervisorRef)
		{
			this.ReportingStepRef = reportingStepRef;
			this.ReportPartExtendedProperties = reportPartExtendedProperties;
			this.SupervisorRef = supervisorRef;
		}

		[DataMember]
		public EntityRef ReportingStepRef;

		[DataMember]
		public Dictionary<string, string> ReportPartExtendedProperties;

		[DataMember]
		public EntityRef SupervisorRef;

		/// <summary>
		/// Sets the priority of the associated order.  Optional - if null, priority is unchanged.
		/// </summary>
		[DataMember]
		public EnumValueInfo Priority;
	}
}
