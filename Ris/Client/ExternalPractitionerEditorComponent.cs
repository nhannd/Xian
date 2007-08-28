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
    public class ExternalPractitionerEditorComponent : NavigatorComponentContainer
    {
        private EntityRef _practitionerRef;
        private ExternalPractitionerDetail _practitionerDetail;

        // return values for staff
        private ExternalPractitionerSummary _practitionerSummary;

        private bool _isNew;

        private ExternalPractitionerDetailsEditorComponent _detailsEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

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
        /// <param name="staffMode"></param>
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

                    this.ValidationStrategy = new AllNodesContainerValidationStrategy();

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

                this.Host.Exit();
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
