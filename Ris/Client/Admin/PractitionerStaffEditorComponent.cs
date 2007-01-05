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
    public class PractitionerStaffEditorComponent : NavigatorComponentContainer
    {
        private bool _isStaffMode;
        private EntityRef<Staff> _staffRef;
        private Staff _staff;
        private EntityRef<Practitioner> _practitionerRef;
        private Practitioner _practitioner;

        private bool _isNew;
        private IStaffAdminService _staffAdminService;
        private IPractitionerAdminService _practitionerAdminService;

        private PractitionerStaffDetailsEditorComponent _detailsEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

        /// <summary>
        /// Constructs an editor to edit a new staff
        /// </summary>
        public PractitionerStaffEditorComponent(bool staffMode)
        {
            _isNew = true;
            _isStaffMode = staffMode;
        }

        /// <summary>
        /// Constructs an editor to edit an existing staff profile
        /// </summary>
        /// <param name="staffRef"></param>
        public PractitionerStaffEditorComponent(EntityRef<Staff> staffRef)
        {
            _staffRef = staffRef;
            _isNew = false;
            _isStaffMode = true;
        }

        /// <summary>
        /// Constructs an editor to edit an existing practitioner profile
        /// </summary>
        /// <param name="practitionerRef"></param>
        public PractitionerStaffEditorComponent(EntityRef<Practitioner> practitionerRef)
        {
            _practitionerRef = practitionerRef;
            _isNew = false;
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

            if (_isStaffMode)
                InitializeStaffEditor();
            else
                InitializePractitionerEditor();

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

        #region Staff helper functions

        private void InitializeStaffEditor()
        {
            if (_isNew)
                _staff = new Staff();
            else
            {
                try
                {
                    _staff = _staffAdminService.LoadStaff(_staffRef, true);
                }
                catch (Exception e)
                {
                    ExceptionHandler.Report(e, this.Host.DesktopWindow);
                }
            }

            this.Pages.Add(new NavigatorPage("Staff", _detailsEditor = new PractitionerStaffDetailsEditorComponent()));
            this.Pages.Add(new NavigatorPage("Staff/Addresses", _addressesSummary = new AddressesSummaryComponent()));
            this.Pages.Add(new NavigatorPage("Staff/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

            this.ValidationStrategy = new AllNodesContainerValidationStrategy();

            _detailsEditor.Staff = _staff;
            _addressesSummary.Subject = _staff.Addresses;
            _phoneNumbersSummary.Subject = _staff.TelephoneNumbers;
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

        #endregion

        #region Practitioner helper functions

        private void InitializePractitionerEditor()
        {
            if (_isNew)
                _practitioner = new Practitioner();
            else
                _practitioner = _practitionerAdminService.LoadPractitioner(_practitionerRef, true);

            this.Pages.Add(new NavigatorPage("Practitioner", _detailsEditor = new PractitionerStaffDetailsEditorComponent()));
            this.Pages.Add(new NavigatorPage("Practitioner/Addresses", _addressesSummary = new AddressesSummaryComponent()));
            this.Pages.Add(new NavigatorPage("Practitioner/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent()));

            this.ValidationStrategy = new AllNodesContainerValidationStrategy();

            _detailsEditor.Practitioner = _practitioner;
            _addressesSummary.Subject = _practitioner.Addresses;
            _phoneNumbersSummary.Subject = _practitioner.TelephoneNumbers;
        }

        private void SavePractitionerChanges()
        {
            if (_isNew)
            {
                _practitionerAdminService.AddPractitioner(_practitioner);
                _practitionerRef = new EntityRef<Practitioner>(_practitioner);
            }
            else
            {
                _practitionerAdminService.UpdatePractitioner(_practitioner);
            }
        }

        #endregion
    }
}
