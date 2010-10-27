#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
	[DataContract]
	public class RegistrationWorklistItemSummary : WorklistItemSummaryBase
	{
		public RegistrationWorklistItemSummary(
			EntityRef procedureRef,
			EntityRef orderRef,
			EntityRef patientRef,
			EntityRef profileRef,
			CompositeIdentifierDetail mrn,
			PersonNameDetail name,
			string accessionNumber,
			EnumValueInfo orderPriority,
			EnumValueInfo patientClass,
			string diagnosticServiceName,
			string procedureName,
			bool procedurePortable,
			EnumValueInfo procedureLaterality,
			DateTime? time)
			:base(
				null,
				procedureRef,
				orderRef,
				patientRef,
				profileRef,
				mrn,
				name,
				accessionNumber,
				orderPriority,
				patientClass,
				diagnosticServiceName,
				procedureName,
				procedurePortable,
				procedureLaterality,
				null,
				time
			)
		{
		}
	}
}
