#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines an interface for providing custom editing pages to be displayed in the staff editor.
    /// </summary>
    public interface IExternalPractitionerEditorPageProvider
    {
        IExternalPractitionerEditorPage[] GetEditorPages(ExternalPractitionerDetail practitionerDetail);
    }

    /// <summary>
    /// Defines an interface to a custom staff editor page.
    /// </summary>
    public interface IExternalPractitionerEditorPage
    {
        Path Path { get; }
        IApplicationComponent GetComponent();

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
        private EntityRef _practitionerRef;
        private ExternalPractitionerDetail _practitionerDetail;

        // return values for staff
        private ExternalPractitionerSummary _practitionerSummary;

        private readonly bool _isNew;

        private ExternalPractitionerDetailsEditorComponent _detailsEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

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
            Platform.GetService<IExternalPractitionerAdminService>(
                delegate(IExternalPractitionerAdminService service)
                {
                    LoadExternalPractitionerEditorFormDataResponse formDataResponse = service.LoadExternalPractitionerEditorFormData(new LoadExternalPractitionerEditorFormDataRequest());

                    string rootPath = SR.TitleExternalPractitioner;
                    this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new ExternalPractitionerDetailsEditorComponent(_isNew)));
                    this.Pages.Add(new NavigatorPage(rootPath + "/Addresses", _addressesSummary = new AddressesSummaryComponent(formDataResponse.AddressTypeChoices)));
                    this.Pages.Add(new NavigatorPage(rootPath + "/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(formDataResponse.PhoneTypeChoices)));

                    this.ValidationStrategy = new AllComponentsValidationStrategy();

                    if (_isNew)
                    {
                        _practitionerDetail = new ExternalPractitionerDetail();
                    }
                    else
                    {
                        LoadExternalPractitionerForEditResponse response = service.LoadExternalPractitionerForEdit(new LoadExternalPractitionerForEditRequest(_practitionerRef));
                        _practitionerRef = response.PractitionerRef;
                        _practitionerDetail = response.PractitionerDetail;
                    }

                    _detailsEditor.ExternalPractitionerDetail = _practitionerDetail;
                    _addressesSummary.Subject = _practitionerDetail.Addresses;
                    _phoneNumbersSummary.Subject = _practitionerDetail.TelephoneNumbers;
                });

            // instantiate all extension pages
            _extensionPages = new List<IExternalPractitionerEditorPage>();
            foreach (IExternalPractitionerEditorPageProvider pageProvider in new ExternalPractitionerEditorPageProviderExtensionPoint().CreateExtensions())
            {
                _extensionPages.AddRange(pageProvider.GetEditorPages(_practitionerDetail));
            }

            // add extension pages to navigator
            // the navigator will start those components if the user goes to that page
            foreach (IStaffEditorPage page in _extensionPages)
            {
                this.Pages.Add(new NavigatorPage(page.Path.LocalizedPath, page.GetComponent()));
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
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
                            UpdateExternalPractitionerResponse response = service.UpdateExternalPractitioner(new UpdateExternalPractitionerRequest(_practitionerRef, _practitionerDetail));
                            _practitionerRef = response.Practitioner.PractitionerRef;
                            _practitionerSummary = response.Practitioner;
                        }
                    });

                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionSaveExternalPractitioner, this.Host.DesktopWindow,
                    delegate()
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
