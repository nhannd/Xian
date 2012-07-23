#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Extended.Common.ProtocollingWorkflow
{
	[DataContract]
	public class StartProtocolRequest : DataContractBase
	{
		public StartProtocolRequest(EntityRef protocolAssignmentStepRef, List<EntityRef> linkedProtocolAssignmentStepRefs, bool shouldClaim, string noteCategory)
		{
			this.ProtocolAssignmentStepRef = protocolAssignmentStepRef;
			this.LinkedProtocolAssignmentStepRefs = linkedProtocolAssignmentStepRefs;
			this.ShouldClaim = shouldClaim;
			this.NoteCategory = noteCategory;
		}

		[DataMember]
		public EntityRef ProtocolAssignmentStepRef;

		[DataMember]
		public List<EntityRef> LinkedProtocolAssignmentStepRefs;

		[DataMember]
		public bool ShouldClaim;

		[DataMember]
		public string NoteCategory;
	}
}
