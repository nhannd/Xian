using System;
using System.Collections;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ContactPersonsSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ContactPersonsSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ContactPersonsSummaryComponent class
    /// </summary>
    [AssociateView(typeof(ContactPersonsSummaryComponentViewExtensionPoint))]
    public class ContactPersonsSummaryComponent : ApplicationComponent
    {
        private IList _contactPersonList;
        private ContactPersonTable _contactPersons;
        private ContactPerson _currentContactPersonSelection;
        private CrudActionModel _contactPersonActionHandler;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonsSummaryComponent()
        {
            _contactPersons = new ContactPersonTable();

            _contactPersonActionHandler = new CrudActionModel();
            _contactPersonActionHandler.Add.SetClickHandler(AddContactPerson);
            _contactPersonActionHandler.Edit.SetClickHandler(UpdateSelectedContactPerson);
            _contactPersonActionHandler.Delete.SetClickHandler(DeleteSelectedContactPerson);

            _contactPersonActionHandler.Add.Enabled = true;
            _contactPersonActionHandler.Edit.Enabled = false;
            _contactPersonActionHandler.Delete.Enabled = false;
        }

        public IList Subject
        {
            get { return _contactPersonList; }
            set { _contactPersonList = value; }
        }

        public override void Start()
        {
            if (_contactPersonList != null)
            {
                foreach (ContactPerson contactPerson in _contactPersonList)
                {
                    _contactPersons.Items.Add(contactPerson);
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

        public ITable ContactPersons
        {
            get { return _contactPersons; }
        }

        public ActionModelNode ContactPersonListActionModel
        {
            get { return _contactPersonActionHandler; }
        }

        public ISelection SelectedContactPerson
        {
            get 
            { 
                return _currentContactPersonSelection == null 
                    ? Selection.Empty 
                    : new Selection(_currentContactPersonSelection); 
            }
            set
            {
                _currentContactPersonSelection = (ContactPerson)value.Item;
                ContactPersonSelectionChanged();
            }
        }

        public void AddContactPerson()
        {
            ContactPerson contactPerson = new ContactPerson();

            ContactPersonEditorComponent editor = new ContactPersonEditorComponent(contactPerson);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddContactPerson);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _contactPersons.Items.Add(contactPerson);
                _contactPersonList.Add(contactPerson);
                this.Modified = true;
            }
        }

        public void UpdateSelectedContactPerson()
        {
            // can occur if user double clicks while holding control
            if (_currentContactPersonSelection == null) return;

            ContactPerson contactPerson = (ContactPerson)_currentContactPersonSelection.Clone();

            ContactPersonEditorComponent editor = new ContactPersonEditorComponent(contactPerson);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateContactPerson);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                ContactPerson toBeRemoved = _currentContactPersonSelection;
                _contactPersons.Items.Remove(toBeRemoved);
                _contactPersonList.Remove(toBeRemoved);

                _contactPersons.Items.Add(contactPerson);
                _contactPersonList.Add(contactPerson);

                this.Modified = true;
            }
        }

        public void DeleteSelectedContactPerson()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedAddress, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary Address otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Address being removed from the PatientProfile
                ContactPerson toBeRemoved = _currentContactPersonSelection;
                _contactPersons.Items.Remove(toBeRemoved);
                _contactPersonList.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

        private void ContactPersonSelectionChanged()
        {
            _contactPersonActionHandler.Edit.Enabled =
                _contactPersonActionHandler.Delete.Enabled = (_currentContactPersonSelection != null);
        }
    }
}
