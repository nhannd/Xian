using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Ris.Services;
using ClearCanvas.Enterprise;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Client.Common;

namespace ClearCanvas.Ris.Client.Admin
{
    public class StaffEditorComponent : NavigatorComponentContainer
    {
        private bool _isStaffMode;
        private EntityRef<Staff> _staffRef;
        private EntityRef<Practitioner> _practitionerRef;
        private Staff _staff;

        private bool _isNew;
        private IStaffAdminService _staffAdminService;
        private IPractitionerAdminService _practitionerAdminService;

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
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        /// <param name="staffRef"></param>
        public StaffEditorComponent(EntityRef<Staff> staffRef)
        {
            _isNew = false;
            _staffRef = staffRef;
            _isStaffMode = true;
        }

        /// <summary>
        /// Constructs an editor to edit an existing practitioner profile
        /// </summary>
        /// <param name="staffRef"></param>
        public StaffEditorComponent(EntityRef<Practitioner> staffRef)
        {
            _isNew = false;
            _practitionerRef = staffRef;
            _isStaffMode = false;
        }

        public bool StaffMode
        {
            get { return _isStaffMode; }
            set { _isStaffMode = value; }
        }

        public override void Start()
        {
            _staffAdminService = ApplicationContext.GetService<IStaffAdminService>();
            _practitionerAdminService = ApplicationContext.GetService<IPractitionerAdminService>();

            if (_isNew)
            {
                if (_isStaffMode)
                    _staff = new Staff();
                else
                    _staff = new Practitioner();
            }
            else
            {
                try
                {
                    if (_isStaffMode)
                        _staff = _staffAdminService.LoadStaff(_staffRef, true);
                    else
                        _staff = _practitionerAdminService.LoadPractitioner(_practitionerRef, true);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            string rootPath = _isStaffMode ? SR.TitleStaff : SR.TitlePractitioner;
            this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new StaffDetailsEditorComponent(_isStaffMode)));
            this.Pages.Add(new NavigatorPage(rootPath + "/Addresses", _addressesSummary = new AddressesSummaryComponent()));
            this.Pages.Add(new NavigatorPage(rootPath + "/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

            this.ValidationStrategy = new AllNodesContainerValidationStrategy();

            _detailsEditor.Staff = _staff;
            _addressesSummary.Subject = _staff.Addresses;
            _phoneNumbersSummary.Subject = _staff.TelephoneNumbers;

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
            }
            else
            {
                try
                {
                    if (_isStaffMode)
                        SaveStaffChanges();
                    else
                        SavePractitionerChanges();

                    this.ExitCode = ApplicationComponentExitCode.Normal;
                }
                catch (ConcurrencyException e)
                {
                    if (_isStaffMode)
                        ExceptionHandler.Report(e, SR.ExceptionConcurrencyStaffNotSaved, this.Host.DesktopWindow);
                    else
                        ExceptionHandler.Report(e, SR.ExceptionConcurrencyPractitionerNotSaved, this.Host.DesktopWindow);

                    this.ExitCode = ApplicationComponentExitCode.Error;
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, SR.ExceptionFailedToSave, this.Host.DesktopWindow);
                    this.ExitCode = ApplicationComponentExitCode.Error;
                }
                this.Host.Exit();
            }
        }

        public override void Cancel()
        {
            base.Cancel();
        }

        private void SaveStaffChanges()
        {
            if (_isNew)
            {
                _staffAdminService.AddStaff(_staff);
                _staffRef = new EntityRef<Staff>(_staff);
            }
            else
            {
                _staffAdminService.UpdateStaff(_staff);
            }
        }

        private void SavePractitionerChanges()
        {
            if (_isNew)
            {
                _practitionerAdminService.AddPractitioner(_staff as Practitioner);
                _staffRef = new EntityRef<Staff>(_staff);
            }
            else
            {
                _practitionerAdminService.UpdatePractitioner(_staff as Practitioner);
            }
        }
    }
}
