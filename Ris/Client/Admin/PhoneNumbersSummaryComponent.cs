using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Admin
{
    public class PhoneNumbersSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PhoneNumbersSummaryComponentViewExtensionPoint))]
    public class PhoneNumbersSummaryComponent : ApplicationComponent
    {
        class PhoneNumberActionHandler : CrudActionHandler
        {
            private PhoneNumbersSummaryComponent _component;

            internal PhoneNumberActionHandler(PhoneNumbersSummaryComponent component)
            {
                _component = component;
            }

            protected override void Add()
            {
                _component.AddPhoneNumber();
            }

            protected override void Edit()
            {
                _component.UpdateSelectedPhoneNumber();
            }

            protected override void Delete()
            {
                _component.DeleteSelectedPhoneNumber();
            }
        }

        private PatientProfile _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<TelephoneNumber> _phoneNumbers;
        private TelephoneNumber _currentPhoneNumberSelection;

        PhoneNumberActionHandler _phoneNumberActionHandler;

        public PhoneNumbersSummaryComponent()
        {
            _phoneNumbers = new TableData<TelephoneNumber>();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();

            _phoneNumbers.AddColumn<string>("Use", delegate(TelephoneNumber pn) { return _patientAdminService.TelephoneUseEnumTable[pn.Use].Value; });
            _phoneNumbers.AddColumn<string>("Equipment", delegate(TelephoneNumber pn) { return _patientAdminService.TelephoneEquipmentEnumTable[pn.Equipment].Value; });
            _phoneNumbers.AddColumn<string>("Country Code", delegate(TelephoneNumber pn) { return pn.CountryCode; });
            _phoneNumbers.AddColumn<string>("Area Code", delegate(TelephoneNumber pn) { return pn.AreaCode; });
            _phoneNumbers.AddColumn<string>("Number", delegate(TelephoneNumber pn) { return pn.Number; });
            _phoneNumbers.AddColumn<string>("Extension", delegate(TelephoneNumber pn) { return pn.Extension; });

            _phoneNumberActionHandler = new PhoneNumberActionHandler(this);
            _phoneNumberActionHandler.AddEnabled = true;
        }

        public PatientProfile Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public ITableData PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        public ActionModelNode PhoneNumberListToolbarActions
        {
            get { return _phoneNumberActionHandler.ToolbarModel; }
        }

        public ActionModelNode PhoneNumberListMenuActions
        {
            get { return _phoneNumberActionHandler.MenuModel; }
        }

        public TelephoneNumber CurrentPhoneNumberSelection
        {
            get { return _currentPhoneNumberSelection; }
            set
            {
                _currentPhoneNumberSelection = value;
                PhoneNumberSelectionChanged();
            }
        }

        public void SetSelectedPhoneNumber(ISelection selection)
        {
            this.CurrentPhoneNumberSelection = (TelephoneNumber)selection.Item;
        }

        private void PhoneNumberSelectionChanged()
        {
            if (_currentPhoneNumberSelection != null)
            {
                _phoneNumberActionHandler.EditEnabled = true;
                _phoneNumberActionHandler.DeleteEnabled = true;
            }
            else
            {
                _phoneNumberActionHandler.EditEnabled = false;
                _phoneNumberActionHandler.DeleteEnabled = false;
            }
        }



        public void AddPhoneNumber()
        {
            TelephoneNumber phoneNumber = TelephoneNumber.New();

            //provide reasonable defaults for following two fields until mapping file modified to nullable
            phoneNumber.CountryCode = "1";
            phoneNumber.Extension = "N/A";

            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Add Phone Number...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _phoneNumbers.Add(phoneNumber);
                _patient.TelephoneNumbers.Add(phoneNumber);
                this.Modified = true;
            }
        }

        public void UpdateSelectedPhoneNumber()
        {
            // can occur if user double clicks while holding control
            if (_currentPhoneNumberSelection == null) return;

            TelephoneNumber phoneNumber = TelephoneNumber.New();
            phoneNumber.CopyFrom(_currentPhoneNumberSelection);
            
            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Update Phone Number...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                TelephoneNumber toBeRemoved = _currentPhoneNumberSelection;
                _phoneNumbers.Remove(toBeRemoved);
                _patient.TelephoneNumbers.Remove(toBeRemoved);

                _phoneNumbers.Add(phoneNumber);
                _patient.TelephoneNumbers.Add(phoneNumber);

                this.Modified = true;
            }
        }

        public void DeleteSelectedPhoneNumber()
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this phone number?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary TelephoneNumber otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong TelephoneNumber being removed from the PatientProfile
                TelephoneNumber toBeRemoved = _currentPhoneNumberSelection;
                _phoneNumbers.Remove(toBeRemoved);
                _patient.TelephoneNumbers.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        public void LoadPhoneNumbersTable()
        {
            if (_patient != null)
            {
                foreach (TelephoneNumber phoneNumber in _patient.TelephoneNumbers)
                {
                    _phoneNumbers.Add(phoneNumber);
                }
            }
        }


    }
}
