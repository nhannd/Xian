#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
    [DataContract]
    public class CompleteOrderDocumentationResponse : DataContractBase
    {

		/// <summary>
		/// Returns the updated procedure plan.
		/// </summary>
        [DataMember]
        public ProcedurePlanDetail ProcedurePlan;

		/// <summary>
		/// Returns the set of interpretation steps that are now scheduled for the procedures in this order.
		/// </summary>
		//JR: this was added for the benefit of the Oto workflow service, that needs to know how to proceed in the workflow.
		[DataMember]
    	public List<EntityRef> InterpretationStepRefs;
    }
}