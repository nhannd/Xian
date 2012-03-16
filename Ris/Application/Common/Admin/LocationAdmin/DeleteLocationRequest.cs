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

namespace ClearCanvas.Ris.Application.Common.Admin.LocationAdmin
{
	[DataContract]
	public class DeleteLocationRequest : DataContractBase
	{
		public DeleteLocationRequest(EntityRef locationRef)
		{
			this.LocationRef = locationRef;
		}

		[DataMember]
		public EntityRef LocationRef;
	}
}
