#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReturnToInterpreterRequest : SaveReportRequest
	{
		public ReturnToInterpreterRequest(EntityRef stepRef)
			: this(stepRef, null, null)
		{
		}

		public ReturnToInterpreterRequest(EntityRef stepRef
			, Dictionary<string, string> reportPartExtendedProperties
			, EntityRef supervisorRef)
			: base(stepRef, reportPartExtendedProperties, supervisorRef)
		{
		}
	}
}
