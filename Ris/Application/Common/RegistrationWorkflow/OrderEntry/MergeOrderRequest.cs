#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using System.Collections.Generic;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class MergeOrderRequest : DataContractBase
	{
		public MergeOrderRequest(List<EntityRef> sourceOrderRefs, EntityRef destinationOrderRef)
		{
			this.SourceOrderRefs = sourceOrderRefs;
			this.DestinationOrderRef = destinationOrderRef;
		}

		[DataMember]
		public List<EntityRef> SourceOrderRefs;

		[DataMember]
		public EntityRef DestinationOrderRef;

		[DataMember]
		public bool DryRun;

		/// <summary>
		/// Validation will always be performed for dry-run.  But if only validation is needed, set this flag to true.  The DryRun flag will then be ignored.
		/// </summary>
		[DataMember]
		public bool ValidationOnly;
	}
}
