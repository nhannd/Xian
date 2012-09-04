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
	public class AttachmentSummary : DataContractBase
	{
		public AttachmentSummary(EnumValueInfo category, StaffSummary attachedBy, DateTime attachedTime, AttachedDocumentSummary document)
		{
			this.Category = category;
			this.AttachedBy = attachedBy;
			this.AttachedTime = attachedTime;
			this.Document = document;
		}

		[DataMember]
		public EnumValueInfo Category;

		[DataMember]
		public StaffSummary AttachedBy;

		[DataMember]
		public DateTime AttachedTime;

		[DataMember]
		public AttachedDocumentSummary Document;
	}
}