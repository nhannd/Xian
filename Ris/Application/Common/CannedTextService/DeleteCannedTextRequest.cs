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

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class DeleteCannedTextRequest : DataContractBase
	{
		public DeleteCannedTextRequest(EntityRef cannedTextRef)
        {
			this.CannedTextRef = cannedTextRef;
        }

        [DataMember]
		public EntityRef CannedTextRef;
	}
}
