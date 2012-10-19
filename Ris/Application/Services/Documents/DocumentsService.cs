#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Application.Common.Documents;

namespace ClearCanvas.Ris.Application.Services.Documents
{
	[ServiceImplementsContract(typeof(IDocumentsService))]
	[ExtensionOf(typeof(ApplicationServiceExtensionPoint))]
	public class DocumentsService : ApplicationServiceBase, IDocumentsService
	{
		[ReadOperation]
		public GetAttachedDocumentFormDataResponse GetAttachedDocumentFormData(GetAttachedDocumentFormDataRequest request)
		{
			return new GetAttachedDocumentFormDataResponse(
				EnumUtils.GetEnumValueList<PatientAttachmentCategoryEnum>(PersistenceContext),
				EnumUtils.GetEnumValueList<OrderAttachmentCategoryEnum>(PersistenceContext));
		}

		[UpdateOperation]
		public UploadResponse Upload(UploadRequest request)
		{
			var tempFile = Path.GetTempFileName();
			try
			{
				// write data to a temp file
				File.WriteAllBytes(tempFile, request.DataContent);

				// create the new document object, and put the remote file
				var args = new AttachedDocumentCreationArgs
				{
					MimeType = request.MimeType,
					FileExtension = request.FileExtension,
					LocalContentFilePath = tempFile
				};

				var document = AttachedDocument.Create(args, AttachmentStore.GetClient());
				PersistenceContext.Lock(document, DirtyState.New);

				PersistenceContext.SynchState();

				var assembler = new AttachedDocumentAssembler();
				return new UploadResponse(assembler.CreateAttachedDocumentSummary(document));

			}
			finally
			{
				File.Delete(tempFile);
			}
		}
	}
}
