#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
    public class PhoneNumbersSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PhoneNumbersSummaryComponentViewExtensionPoint))]
    public class PhoneNumbersSummaryComponent : ApplicationComponent
    {
        private IList<TelephoneDetail> _phoneNumberList;
        private TelephoneNumberTable _phoneNumbers;
        private TelephoneDetail _currentPhoneNumberSelection;
        private CrudActionModel _phoneNumberActionHandler;
        private IList<EnumValueInfo> _phoneTypeChoices;

        public PhoneNumbersSummaryComponent(IList<EnumValueInfo> phoneTypeChoices)
        {
            _phoneNumbers = new TelephoneNumberTable();

            _phoneNumberActionHandler = new CrudActionModel();
            _phoneNumberActionHandler.Add.SetClickHandler(AddPhoneNumber);
            _phoneNumberActionHandler.Edit.SetClickHandler(UpdateSelectedPhoneNumber);
            _phoneNumberActionHandler.Delete.SetClickHandler(DeleteSelectedPhoneNumber);

            _phoneNumberActionHandler.Add.Enabled = true;
            _phoneNumberActionHandler.Edit.Enabled = false;
            _phoneNumberActionHandler.Delete.Enabled = false;

            _phoneTypeChoices = phoneTypeChoices;
        }

        public IList<TelephoneDetail> Subject
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
            phoneNumber.Type = _phoneTypeChoices[0];

            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber, _phoneTypeChoices);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddPhoneNumber);
            if (exitCode == ApplicationComponentExitCode.Accepted)
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

            PhoneNumberEditorComponent editor = new PhoneNumberEditorComponent(phoneNumber, _phoneTypeChoices);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdatePhoneNumber);
            if (exitCode == ApplicationComponentExitCode.Accepted)
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
