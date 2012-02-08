#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using System.Collections;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Documents;
using System.Collections.Generic;
using System.IO;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extension point for views onto <see cref="AttachDocumentComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class AttachDocumentComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// AttachDocumentComponent class
	/// </summary>
	[AssociateView(typeof(AttachDocumentComponentViewExtensionPoint))]
	public class AttachDocumentComponent : ApplicationComponent
	{
		private readonly AttachmentSite _site;

		public AttachDocumentComponent(AttachmentSite site)
		{
			_site = site;
		}

		public AttachedDocumentSummary Document { get; private set; }

		public override void Start()
		{
			base.Start();

			Platform.GetService<IDocumentsService>(service =>
				{
					var response = service.GetAttachedDocumentFormData(new GetAttachedDocumentFormDataRequest());
					CategoryChoices = _site == AttachmentSite.Patient
									? response.PatientAttachmentCategoryChoices
									: response.OrderAttachmentCategoryChoices;
				});
		}



		#region Presentation Model

		[ValidateNotNull]
		[ValidateRegex(@"\.pdf$|\.PDF$", Message = "MessageAttachmentMustBePdf")]
		public string FilePath { get; set; }

		public IList CategoryChoices { get; private set; }

		[ValidateNotNull]
		public EnumValueInfo Category { get; set; }


		public void BrowseFiles()
		{
			try
			{
				var args = new FileDialogCreationArgs();
				args.Filters.Add(new FileExtensionFilter("*.pdf", "PDF files (*.pdf)"));

				var result = this.Host.DesktopWindow.ShowOpenFileDialogBox(args);
				if (result.Action == DialogBoxAction.Ok)
				{
					this.FilePath = result.FileName;
					NotifyPropertyChanged("FilePath");
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Accept()
		{
			if(this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				this.Document = UploadFile();
				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void Cancel()
		{
			this.Exit(ApplicationComponentExitCode.None);
		}

		#endregion

		private AttachedDocumentSummary UploadFile()
		{
			AttachedDocumentSummary result = null;
			var data = File.ReadAllBytes(this.FilePath);
			Platform.GetService<IDocumentsService>(service =>
			{
				var response = service.Upload(new UploadRequest("pdf", "pdf", data));
				result = response.Document;
			});
			return result;
		}
	}
}
