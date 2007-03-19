using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client
{
    public class PhoneNumbersSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PhoneNumbersSummaryComponentViewExtensionPoint))]
    public class PhoneNumbersSummaryComponent : ApplicationComponent
    {
        private IList _phoneNumberList;
        private TelephoneNumberTable _phoneNumbers;
        private TelephoneDetail _currentPhoneNumberSelection;
        private CrudActionModel _phoneNumberActionHandler;

        public PhoneNumbersSummaryComponent()
        {
            _phoneNumbers = new TelephoneNumberTable();

            _phoneNumberActionHandler = new CrudActionModel();
            _phoneNumberActionHandler.Add.SetClickHandler(AddPhoneNumber);
            _phoneNumberActionHandler.Edit.SetClickHandler(UpdateSelectedPhoneNumber);
            _phoneNumberActionHandler.Delete.SetClickHandler(DeleteSelectedPhoneNumber);

            _phoneNumberActionHandler.Add.Enabled = true;
            _phoneNumberActionHandler.Edit.Enabled = false;
            _phoneNumberActionHandler.Delete.Enabled = false;
        }

        public IList Subject
        {
            get { return _phoneNumberList; }
            set { _phoneNumberList = value; }
        }

        public override void Start()
        {
            if (_phoneNumberList != null)
            {
                foreach (TelephoneDetail phoneNumber in _phoneNumberList)
                {
                    _phoneNumbers.Items.Add(phoneNumber);
                }
            }

            base.Start();
        }

        #region Presentation Model

        public ITable PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        public ActionModelNode PhoneNumberListActionModel
        {
            get { return _phoneNumberActionHandler; }
        }

        public ISelection SelectedPhoneNumber
        {
            get { return _currentPhoneNumberSelection == null ? Selection.Empty : new Selection(_currentPhoneNumberSelection); }
            set
            {
                _currentPhoneNumberSelection = (TelephoneDetail)value.Item;
                PhoneNumberSelectionChanged();
            }
        }

        public void AddPhoneNumber()
        {
            TelephoneDetail phoneNumber = new TelephoneDetail();

            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddPhoneNumber);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _phoneNumbers.Items.Add(phoneNumber);
                _phoneNumberList.Add(phoneNumber);
                this.Modified = true;
            }
        }

        public void UpdateSelectedPhoneNumber()
        {
            // can occur if user double clicks while holding control
            if (_currentPhoneNumberSelection == null) return;

            TelephoneDetail phoneNumber = (TelephoneDetail)_currentPhoneNumberSelection.Clone();
            
            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdatePhoneNumber);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                TelephoneDetail toBeRemoved = _currentPhoneNumberSelection;
                _phoneNumbers.Items.Remove(toBeRemoved);
                _phoneNumberList.Remove(toBeRemoved);

                _phoneNumbers.Items.Add(phoneNumber);
                _phoneNumberList.Add(phoneNumber);

                this.Modified = true;
            }
        }

        public void DeleteSelectedPhoneNumber()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedPhoneNumber, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary TelephoneNumber otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong TelephoneNumber being removed from the PatientProfile
                TelephoneDetail toBeRemoved = _currentPhoneNumberSelection;
                _phoneNumbers.Items.Remove(toBeRemoved);
                _phoneNumberList.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

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

    }
}
