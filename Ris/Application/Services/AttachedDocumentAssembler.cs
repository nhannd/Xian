#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Application.Services
{
	public class AttachedDocumentAssembler
	{
		public AttachedDocumentSummary CreateAttachedDocumentSummary(AttachedDocument doc)
		{
			var summary = new AttachedDocumentSummary();

			UpdateAttachedDocumentSummary(doc, summary);

			return summary;
		}

		public void UpdateAttachedDocumentSummary(AttachedDocument doc, AttachedDocumentSummary summary)
		{
			summary.DocumentRef = doc.GetRef();
			summary.CreationTime = doc.CreationTime;
			summary.ReceivedTime = doc.DocumentReceivedTime;
			summary.MimeType = doc.MimeType;
			summary.ContentUrl = doc.ContentUrl;
			summary.FileExtension = doc.FileExtension;
			summary.DocumentHeaders = new Dictionary<string, string>(doc.DocumentHeaders);
			summary.DocumentTypeName = doc.DocumentTypeName;
		}
	}
}
