#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class TimeShiftOrderRequest : DataContractBase
	{
		public TimeShiftOrderRequest(EntityRef orderRef, int numberOfMinutes)
		{
			OrderRef = orderRef;
			NumberOfMinutes = numberOfMinutes;
		}

		/// <summary>
		/// Specifies the order to shift.
		/// </summary>
		[DataMember]
		public EntityRef OrderRef;

		/// <summary>
		/// Specifies the number of minutes by which to shift the order in time - may be positive or negative.
		/// </summary>
		[DataMember]
		public int NumberOfMinutes;

	}
}
