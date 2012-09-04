#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow
{
	[DataContract]
	public class DiscardProtocolRequest : UpdateProtocolRequest
	{
		public DiscardProtocolRequest(EntityRef protocolStepRef, EntityRef reassignToStaff)
			: base(protocolStepRef, null, null)
		{
			this.ReassignToStaff = reassignToStaff;
		}

		public DiscardProtocolRequest(EntityRef protocolStepRef)
			: this(protocolStepRef, null)
		{
		}

		[DataMember]
		public EntityRef ReassignToStaff;
	}
}
