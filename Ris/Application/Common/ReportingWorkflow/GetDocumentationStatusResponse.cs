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

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class GetDocumentationStatusResponse : DataContractBase
	{
		public GetDocumentationStatusResponse(bool isIncomplete, string reason)
		{
			this.IsIncomplete = isIncomplete;
			this.Reason = reason;
		}

		[DataMember]
		public bool IsIncomplete;

		[DataMember]
		public string Reason;
	}
}