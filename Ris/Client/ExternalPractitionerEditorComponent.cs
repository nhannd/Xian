#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Defines an interface for providing custom editing pages to be displayed in the practitioner editor.
	/// </summary>
	public interface IExternalPractitionerEditorPageProvider : IExtensionPageProvider<IExternalPractitionerEditorPage, IExternalPractitionerEditorContext>
	{
	}

	/// <summary>
	/// Defines an interface for providing a custom editor page with access to the editor
	/// context.
	/// </summary>
	public interface IExternalPractitionerEditorContext
	{
		EntityRef PractitionerRef { get; }

		IDictionary<string, string> PractitionerExtendedProperties { get; }
	}

	/// <summary>
	/// Defines an interface to a custom practitioner editor page.
	/// </summary>
	public interface IExternalPractitionerEditorPage : IExtensionPage
	{
		void Save();
	}

	/// <summary>
	/// Defines an extension point for adding custom pages to the staff editor.
	/// </summary>
	public class ExternalPractitionerEditorPageProviderExtensionPoint : ExtensionPoint<IExternalPractitionerEditorPageProvider>
	{
	}


	public class ExternalPractitionerEditorComponent : NavigatorComponentContainer
	{
		#region EditorContext

		class EditorContext : IExternalPractitionerEditorContext
		{
			private readonly ExternalPractitionerEditorComponent _owner;

			public EditorContext(ExternalPractitionerEditorComponent owner)
			{
				_owner = owner;
			}

			public EntityRef PractitionerRef
			{
				get { return _owner._practitionerRef; }
			}

			public IDictionary<string, string> PractitionerExtendedProperties
			{
				get { return _owner._practitionerDetail.ExtendedProperties; }
			}
		}

		#endregion

		private EntityRef _practitionerRef;
		private ExternalPractitionerDetail _practitionerDetail;

		// return values for staff
		private ExternalPractitionerSummary _practitionerSummary;

		private readonly bool _isNew;

		private ExternalPractitionerDetailsEditorComponent _detailsEditor;
		private ExternalPractitionerContactPointSummaryComponent _contactPointSummaryComponent;

		private List<IExternalPractitionerEditorPage> _extensionPages;

		/// <summary>
		/// Constructs an editor to edit a new staff
		/// </summary>
		public ExternalPractitionerEditorComponent()
		{
			_isNew = true;
		}

		/// <summary>
		/// Constructs an editor to edit an existing staff profile
		/// </summary>
		/// <param name="reference"></param>
		public ExternalPractitionerEditorComponent(EntityRef reference)
		{
			_isNew = false;
			_practitionerRef = reference;
		}

		/// <summary>
		/// Gets summary of staff that was added or edited
		/// </summary>
		public ExternalPractitionerSummary ExternalPractitionerSummary
		{
			get { return _practitionerSummary; }
		}

		public override void Start()
		{
			LoadExternalPractitionerEditorFormDataResponse formDataResponse = null;

			Platform.GetService<IExternalPractitionerAdminService>(
				delegate(IExternalPractitionerAdminService service)
				{
					formDataResponse = service.LoadExternalPractitionerEditorFormData(new LoadExternalPractitionerEditorFormDataRequest());

					if (_isNew)
					{
						_practitionerDetail = new ExternalPractitionerDetail();
					}
					else
					{
						LoadExternalPractitionerForEditResponse response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
						_practitionerRef = response.PractitionerDetail.PractitionerRef;
						_practitionerDetail = response.PractitionerDetail;
					}
				});

			_contactPointSummaryComponent = new ExternalPractitionerContactPointSummaryComponent(_practitionerRef,
				formDataResponse.AddressTypeChoices, 
                formDataResponse.PhoneTypeChoices, 
                formDataResponse.ResultCommunicationModeChoices, 
                Formatting.PersonNameFormat.Format(_practitionerDetail.Name),
                false);
			_contactPointSummaryComponent.SetModifiedOnListChange = true;

			string rootPath = SR.TitleExternalPractitioner;
			this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new ExternalPractitionerDetailsEditorComponent(_isNew)));
			this.Pages.Add(new NavigatorPage(rootPath + "/" + SR.TitleContactPoints, _contactPointSummaryComponent));

			this.ValidationStrategy = new AllComponentsValidationStrategy();

			_detailsEditor.ExternalPractitionerDetail = _practitionerDetail;
			_practitionerDetail.ContactPoints.ForEach(delegate(ExternalPractitionerContactPointDetail p)
													  {
														  _contactPointSummaryComponent.Subject.Add(p);
													  });

			// instantiate all extension pages
			_extensionPages = new List<IExternalPractitionerEditorPage>();
			foreach (IExternalPractitionerEditorPageProvider pageProvider in new ExternalPractitionerEditorPageProviderExtensionPoint().CreateExtensions())
			{
				_extensionPages.AddRange(pageProvider.GetPages(new EditorContext(this)));
			}

			// add extension pages to navigator
			// the navigator will start those components if the user goes to that page
			foreach (IExternalPractitionerEditorPage page in _extensionPages)
			{
				this.Pages.Add(new NavigatorPage(page.Path.LocalizedPath, page.GetComponent()));
			}

			base.Start();
		}

		public override void Accept()
		{
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			try
			{
				_practitionerDetail.ContactPoints.Clear();
				foreach (ExternalPractitionerContactPointDetail detail in _contactPointSummaryComponent.Subject)
				{
					_practitionerDetail.ContactPoints.Add(detail);
				}

				// give extension pages a chance to save data prior to commit
				_extensionPages.ForEach(delegate(IExternalPractitionerEditorPage page) { page.Save(); });

				Platform.GetService<IExternalPractitionerAdminService>(
					delegate(IExternalPractitionerAdminService service)
					{
						if (_isNew)
						{
							AddExternalPractitionerResponse response = service.AddExternalPractitioner(new AddExternalPractitionerRequest(_practitionerDetail));
							_practitionerRef = response.Practitioner.PractitionerRef;
							_practitionerSummary = response.Practitioner;
						}
						else
						{
							UpdateExternalPractitionerResponse response = service.UpdateExternalPractitioner(new UpdateExternalPractitionerRequest(_practitionerDetail));
							_practitionerRef = response.Practitioner.PractitionerRef;
							_practitionerSummary = response.Practitioner;
						}
					});

				this.Exit(ApplicationComponentExitCode.Accepted);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, SR.ExceptionSaveExternalPractitioner, this.Host.DesktopWindow,
					delegate
					{
						this.ExitCode = ApplicationComponentExitCode.Error;
						this.Host.Exit();
					});
			}
		}

		public override void Cancel()
		{
			base.Cancel();
		}
	}
}
