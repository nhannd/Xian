using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.Admin.StaffAdmin;
using ClearCanvas.Ris.Application.Common.Admin.PractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
    public class StaffEditorComponent : NavigatorComponentContainer
    {
        private bool _isStaffMode;
        private EntityRef _staffRef;
        private EntityRef _practitionerRef;
        private StaffDetail _staffDetail;
        private PractitionerDetail _practitionerDetail;

        // return values for staff/practitioners
        private StaffSummary _staffSummary;
        private PractitionerSummary _practitionerSummary;

        private bool _isNew;

        private StaffDetailsEditorComponent _detailsEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

        /// <summary>
        /// Constructs an editor to edit a new staff
        /// </summary>
        public StaffEditorComponent(bool staffMode)
        {
            _isNew = true;
            _isStaffMode = staffMode;
        }

        /// <summary>
        /// Constructs an editor to edit an existing staff/practitioner profile
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="staffMode"></param>
        public StaffEditorComponent(EntityRef reference, bool staffMode)
        {
            _isNew = false;
            _isStaffMode = staffMode;


            if (_isStaffMode)
                _staffRef = reference;
            else
                _practitionerRef = reference;                
        
        }

        public bool StaffMode
        {
            get { return _isStaffMode; }
            set { _isStaffMode = value; }
        }

        /// <summary>
        /// Gets summary of staff that was added or edited
        /// </summary>
        public StaffSummary StaffSummary
        {
            get { return _staffSummary; }
        }

        /// <summary>
        /// Gets summary of practitioner that was added or edited
        /// </summary>
        public PractitionerSummary PractitionerSummary
        {
            get { return _practitionerSummary; }
        }

        public override void Start()
        {
            try
            {
                if (_isStaffMode)
                {
                    Platform.GetService<IStaffAdminService>(
                        delegate(IStaffAdminService service)
                        {
                            LoadStaffEditorFormDataResponse formDataResponse = service.LoadStaffEditorFormData(new LoadStaffEditorFormDataRequest());

                            string rootPath = _isStaffMode ? SR.TitleStaff : SR.TitlePractitioner;
                            this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new StaffDetailsEditorComponent(_isStaffMode)));
                            this.Pages.Add(new NavigatorPage(rootPath + "/Addresses", _addressesSummary = new AddressesSummaryComponent(formDataResponse.AddressTypeChoices)));
                            this.Pages.Add(new NavigatorPage(rootPath + "/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(formDataResponse.PhoneTypeChoices)));

                            this.ValidationStrategy = new AllNodesContainerValidationStrategy();

                            if (_isNew)
                            {
                                _staffDetail = new StaffDetail();
                            }
                            else
                            {
                                LoadStaffForEditResponse response = service.LoadStaffForEdit(new LoadStaffForEditRequest(_staffRef));
                                _staffRef = response.StaffRef;
                                _staffDetail = response.StaffDetail;
                            }

                            _detailsEditor.StaffDetail = _staffDetail;
                            _addressesSummary.Subject = _staffDetail.Addresses;
                            _phoneNumbersSummary.Subject = _staffDetail.TelephoneNumbers;
                        });
                }
                else
                {
                    Platform.GetService<IPractitionerAdminService>(
                        delegate(IPractitionerAdminService service)
                        {
                            LoadPractitionerEditorFormDataResponse formDataResponse = service.LoadPractitionerEditorFormData(new LoadPractitionerEditorFormDataRequest());

                            string rootPath = _isStaffMode ? SR.TitleStaff : SR.TitlePractitioner;
                            this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new StaffDetailsEditorComponent(_isStaffMode)));
                            this.Pages.Add(new NavigatorPage(rootPath + "/Addresses", _addressesSummary = new AddressesSummaryComponent(formDataResponse.AddressTypeChoices)));
                            this.Pages.Add(new NavigatorPage(rootPath + "/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(formDataResponse.PhoneTypeChoices)));

                            this.ValidationStrategy = new AllNodesContainerValidationStrategy();

                            if (_isNew)
                            {
                                _practitionerDetail = new PractitionerDetail();
                            }
                            else
                            {
                                LoadPractitionerForEditResponse response = service.LoadPractitionerForEdit(new LoadPractitionerForEditRequest(_practitionerRef));
                                _practitionerRef = response.PractitionerRef;
                                _practitionerDetail = response.PractitionerDetail;
                            }

                            _detailsEditor.PractitionerDetail = _practitionerDetail;
                            _addressesSummary.Subject = _practitionerDetail.Addresses;
                            _phoneNumbersSummary.Subject = _practitionerDetail.TelephoneNumbers;
                        });
                }

            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
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
                if (_isStaffMode)
                {
                    Platform.GetService<IStaffAdminService>(
                        delegate(IStaffAdminService service)
                        {
                            if (_isNew)
                            {
                                AddStaffResponse response = service.AddStaff(new AddStaffRequest(_staffDetail));
                                _staffRef = response.Staff.StaffRef;
                                _staffSummary = response.Staff;
                            }
                            else
                            {
                                UpdateStaffResponse response = service.UpdateStaff(new UpdateStaffRequest(_staffRef, _staffDetail));
                                _staffRef = response.Staff.StaffRef;
                                _staffSummary = response.Staff;
                            }
                        });
                }
                else
                {
                    Platform.GetService<IPractitionerAdminService>(
                        delegate(IPractitionerAdminService service)
                        {
                            if (_isNew)
                            {
                                AddPractitionerResponse response = service.AddPractitioner(new AddPractitionerRequest(_practitionerDetail));
                                _practitionerRef = response.Practitioner.StaffRef;
                                _practitionerSummary = response.Practitioner;
                            }
                            else
                            {
                                UpdatePractitionerResponse response = service.UpdatePractitioner(new UpdatePractitionerRequest(_practitionerRef, _practitionerDetail));
                                _practitionerRef = response.Practitioner.StaffRef;
                                _practitionerSummary = response.Practitioner;
                            }
                        });
                }

                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }

            this.Host.Exit();
        }

        public override void Cancel()
        {
            base.Cancel();
        }
    }
}
