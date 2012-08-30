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

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class LoadDiagnosticServicePlanResponse : DataContractBase
	{
		public LoadDiagnosticServicePlanResponse(DiagnosticServicePlanDetail plan)
		{
			this.DiagnosticServicePlan = plan;
		}

		[DataMember]
		public DiagnosticServicePlanDetail DiagnosticServicePlan;
	}
}
