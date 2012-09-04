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

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class VisitLocationDetail : DataContractBase
	{
		[DataMember]
		public LocationSummary Location;

		[DataMember]
		public string Room;

		[DataMember]
		public string Bed;

		[DataMember]
		public EnumValueInfo Role;

		[DataMember]
		public DateTime? StartTime;

		[DataMember]
		public DateTime? EndTime;
	}
}
