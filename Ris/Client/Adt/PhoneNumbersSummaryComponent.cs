using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class PhoneNumbersSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PhoneNumbersSummaryComponentViewExtensionPoint))]
    public class PhoneNumbersSummaryComponent : ApplicationComponent
    {
        private PatientProfile _patient;
        private IPatientAdminService _patientAdminService;
        private Table<TelephoneNumber> _phoneNumbers;
        private TelephoneNumber _currentPhoneNumberSelection;

        private CrudActionModel _phoneNumberActionHandler;

        private TelephoneEquipmentEnumTable _phoneEquipments;
        private TelephoneUseEnumTable _phoneUses;

        public PhoneNumbersSummaryComponent()
        {
            _phoneNumbers = new Table<TelephoneNumber>();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
            _phoneEquipments = _patientAdminService.GetTelephoneEquipmentEnumTable();
            _phoneUses = _patientAdminService.GetTelephoneUseEnumTable();

            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Type",
                delegate(TelephoneNumber t)
                {
                    return string.Format("{0} {1}",
                        _phoneUses[t.Use].Value,
                        t.Equipment == TelephoneEquipment.CP ? _phoneEquipments[t.Equipment].Value : "");
                }, 
                0.1f)); 
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Number", 
                delegate(TelephoneNumber pn) { return pn.Format(); },
                0.6f));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Valid From", 
                delegate(TelephoneNumber pn) { return Format.Date(pn.ValidRange.From); }, 
                0.15f));
            _phoneNumbers.Columns.Add(new TableColumn<TelephoneNumber, string>("Valid Until", 
                delegate(TelephoneNumber pn) { return Format.Date(pn.ValidRange.Until); }, 
                0.15f));

            _phoneNumberActionHandler = new CrudActionModel();
            _phoneNumberActionHandler.Add.Handler = AddPhoneNumber;
            _phoneNumberActionHandler.Edit.Handler = UpdateSelectedPhoneNumber;
            _phoneNumberActionHandler.Delete.Handler = DeleteSelectedPhoneNumber;

            _phoneNumberActionHandler.Add.Enabled = true;
        }

        public PatientProfile Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public ITable PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        public ActionModelNode PhoneNumberListActionModel
        {
            get { return _phoneNumberActionHandler; }
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
                _phoneNumberActionHandler.Edit.Enabled = true;
                _phoneNumberActionHandler.Delete.Enabled = true;
            }
            else
            {
                _phoneNumberActionHandler.Edit.Enabled = false;
                _phoneNumberActionHandler.Delete.Enabled = false;
            }
        }



        public void AddPhoneNumber()
        {
            TelephoneNumber phoneNumber = TelephoneNumber.New();

            //provide reasonable defaults for following two fields until mapping file modified to nullable
            phoneNumber.CountryCode = "1";
            phoneNumber.Extension = "N/A";

            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Add Phone Number...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _phoneNumbers.Items.Add(phoneNumber);
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
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Update Phone Number...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                TelephoneNumber toBeRemoved = _currentPhoneNumberSelection;
                _phoneNumbers.Items.Remove(toBeRemoved);
                _patient.TelephoneNumbers.Remove(toBeRemoved);

                _phoneNumbers.Items.Add(phoneNumber);
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
                _phoneNumbers.Items.Remove(toBeRemoved);
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
                    _phoneNumbers.Items.Add(phoneNumber);
                }
            }
        }


    }
}
