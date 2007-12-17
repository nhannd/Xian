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
            if (exitCode == ApplicationComponentExitCode.Accepted)
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
            if (exitCode == ApplicationComponentExitCode.Accepted)
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
