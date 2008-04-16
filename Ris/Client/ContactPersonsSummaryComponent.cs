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
        private IList<ContactPersonDetail> _contactPersonList;
        private ContactPersonTable _contactPersons;
        private ContactPersonDetail _currentContactPersonSelection;
        private CrudActionModel _contactPersonActionHandler;

        private IList<EnumValueInfo> _contactTypeChoices;
        private IList<EnumValueInfo> _contactRelationshipChoices;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonsSummaryComponent(IList<EnumValueInfo> contactTypeChoices, IList<EnumValueInfo> contactRelationshipChoices)
        {
            _contactPersons = new ContactPersonTable();

            _contactPersonActionHandler = new CrudActionModel();
            _contactPersonActionHandler.Add.SetClickHandler(AddContactPerson);
            _contactPersonActionHandler.Edit.SetClickHandler(UpdateSelectedContactPerson);
            _contactPersonActionHandler.Delete.SetClickHandler(DeleteSelectedContactPerson);

            _contactPersonActionHandler.Add.Enabled = true;
            _contactPersonActionHandler.Edit.Enabled = false;
            _contactPersonActionHandler.Delete.Enabled = false;

            _contactTypeChoices = contactTypeChoices;
            _contactRelationshipChoices = contactRelationshipChoices;
        }

        public IList<ContactPersonDetail> Subject
        {
            get { return _contactPersonList; }
            set { _contactPersonList = value; }
        }

        public override void Start()
        {
            if (_contactPersonList != null)
            {
                foreach (ContactPersonDetail contactPerson in _contactPersonList)
                {
                    _contactPersons.Items.Add(contactPerson);
                }
            }
            base.Start();
        }

        public override void Stop()
        {
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
                return new Selection(_currentContactPersonSelection); 
            }
            set
            {
                _currentContactPersonSelection = (ContactPersonDetail)value.Item;
                ContactPersonSelectionChanged();
            }
        }

        public void AddContactPerson()
        {
            try
            {
                ContactPersonDetail contactPerson = new ContactPersonDetail();

                ContactPersonEditorComponent editor = new ContactPersonEditorComponent(contactPerson, _contactTypeChoices, _contactRelationshipChoices);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddContactPerson);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _contactPersons.Items.Add(contactPerson);
                    _contactPersonList.Add(contactPerson);
                    this.Modified = true;
                }

            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedContactPerson()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_currentContactPersonSelection == null) return;

                ContactPersonDetail contactPerson = (ContactPersonDetail)_currentContactPersonSelection.Clone();

                ContactPersonEditorComponent editor = new ContactPersonEditorComponent(contactPerson, _contactTypeChoices, _contactRelationshipChoices);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateContactPerson);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    // delete and re-insert to ensure that TableView updates correctly
                    ContactPersonDetail toBeRemoved = _currentContactPersonSelection;
                    _contactPersons.Items.Remove(toBeRemoved);
                    _contactPersonList.Remove(toBeRemoved);

                    _contactPersons.Items.Add(contactPerson);
                    _contactPersonList.Add(contactPerson);

                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DeleteSelectedContactPerson()
        {
            if (this.Host.ShowMessageBox(SR.MessageConfirmDeleteSelectedAddress, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary Address otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Address being removed from the PatientProfile
                ContactPersonDetail toBeRemoved = _currentContactPersonSelection;
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
