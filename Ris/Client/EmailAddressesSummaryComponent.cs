using System;
using System.Collections;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="EmailAddressSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class EmailAddressSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// EmailAddressSummaryComponent class
    /// </summary>
    [AssociateView(typeof(EmailAddressSummaryComponentViewExtensionPoint))]
    public class EmailAddressesSummaryComponent : ApplicationComponent
    {
        private IList<EmailAddressDetail> _emailAddressList;
        private EmailAddressTable _emailAddresses;
        private EmailAddressDetail _currentEmailAddressSelection;
        private CrudActionModel _emailAddressActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public EmailAddressesSummaryComponent()
        {
            _emailAddresses = new EmailAddressTable();

            _emailAddressActionHandler = new CrudActionModel();
            _emailAddressActionHandler.Add.SetClickHandler(AddEmailAddress);
            _emailAddressActionHandler.Edit.SetClickHandler(UpdateSelectedEmailAddress);
            _emailAddressActionHandler.Delete.SetClickHandler(DeleteSelectedEmailAddress);

            _emailAddressActionHandler.Add.Enabled = true;
            _emailAddressActionHandler.Edit.Enabled = false;
            _emailAddressActionHandler.Delete.Enabled = false;

        }

        public IList<EmailAddressDetail> Subject
        {
            get { return _emailAddressList; }
            set { _emailAddressList = value; }
        }

        public override void Start()
        {
            if (_emailAddressList != null)
            {
                foreach (EmailAddressDetail emailAddress in _emailAddressList)
                {
                    _emailAddresses.Items.Add(emailAddress);
                }
            }
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        #region Presentation Model

        public ITable EmailAddresses
        {
            get { return _emailAddresses; }
        }

        public ActionModelNode EmailAddressListActionModel
        {
            get { return _emailAddressActionHandler; }
        }

        public ISelection SelectedEmailAddress
        {
            get { return _currentEmailAddressSelection == null ? Selection.Empty : new Selection(_currentEmailAddressSelection); }
            set
            {
                _currentEmailAddressSelection = (EmailAddressDetail)value.Item;
                EmailAddressSelectionChanged();
            }
        }

        public void AddEmailAddress()
        {
            EmailAddressDetail emailAddress = new EmailAddressDetail();

            EmailAddressEditorComponent editor = new EmailAddressEditorComponent(emailAddress);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddEmailAddress);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _emailAddresses.Items.Add(emailAddress);
                _emailAddressList.Add(emailAddress);
                this.Modified = true;
            }
        }

        public void UpdateSelectedEmailAddress()
        {
            // can occur if user double clicks while holding control
            if (_currentEmailAddressSelection == null) return;

            EmailAddressDetail emailAddress = (EmailAddressDetail)_currentEmailAddressSelection.Clone();

            EmailAddressEditorComponent editor = new EmailAddressEditorComponent(emailAddress);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateEmailAddress);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                EmailAddressDetail toBeRemoved = _currentEmailAddressSelection;
                _emailAddresses.Items.Remove(toBeRemoved);
                _emailAddressList.Remove(toBeRemoved);

                _emailAddresses.Items.Add(emailAddress);
                _emailAddressList.Add(emailAddress);

                this.Modified = true;
            }
        }

        public void DeleteSelectedEmailAddress()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedAddress, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary Address otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Address being removed from the PatientProfile
                EmailAddressDetail toBeRemoved = _currentEmailAddressSelection;
                _emailAddresses.Items.Remove(toBeRemoved);
                _emailAddressList.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

        private void EmailAddressSelectionChanged()
        {
            _emailAddressActionHandler.Edit.Enabled =
                _emailAddressActionHandler.Delete.Enabled = (_currentEmailAddressSelection != null);
        }
    }
}
