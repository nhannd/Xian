#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common
{
	[DataContract]
	public class AttachedDocumentSummary : DataContractBase
	{
		[DataMember]
		public EntityRef DocumentRef;

		[DataMember]
		public DateTime? CreationTime;

		[DataMember]
		public DateTime? ReceivedTime;

		[DataMember]
		public string MimeType;

		[DataMember]
		public string FileExtension;

		[DataMember]
		public string ContentUrl;

		[DataMember]
		public Dictionary<string, string> DocumentHeaders;

		[DataMember]
		public string DocumentTypeName;
	}
}
