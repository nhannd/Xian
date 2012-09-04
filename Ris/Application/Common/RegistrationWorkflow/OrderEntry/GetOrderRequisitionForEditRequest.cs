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

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry
{
	[DataContract]
	public class GetOrderRequisitionForEditRequest : DataContractBase
	{
		public GetOrderRequisitionForEditRequest()
		{
		}

		/// <summary>
		/// Identifies the order requistion to edit.  Either <see cref="OrderRef"/> or <see cref="ProcedureRef"/> must be specified.
		/// </summary>
		[DataMember]
		public EntityRef OrderRef;

		/// <summary>
		/// Identifies the order requistion to edit.  Either <see cref="OrderRef"/> or <see cref="ProcedureRef"/> must be specified.
		/// </summary>
		[DataMember]
		public EntityRef ProcedureRef;
	}
}
