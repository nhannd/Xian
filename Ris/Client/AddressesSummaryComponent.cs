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
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Common.Utilities;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Client
{
    public class AddressesSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(AddressesSummaryComponentViewExtensionPoint))]
    public class AddressesSummaryComponent : ApplicationComponent
    {
        private IList<AddressDetail> _addressList;
        private AddressTable _addresses;
        private AddressDetail _currentAddressSelection;
        private CrudActionModel _addressActionHandler;
        private IList<EnumValueInfo> _addressTypes;

        public AddressesSummaryComponent(IList<EnumValueInfo> addressTypes)
        {
            _addresses = new AddressTable();

            _addressActionHandler = new CrudActionModel();
            _addressActionHandler.Add.SetClickHandler(AddAddress);
            _addressActionHandler.Edit.SetClickHandler(UpdateSelectedAddress);
            _addressActionHandler.Delete.SetClickHandler(DeleteSelectedAddress);

            _addressActionHandler.Add.Enabled = true;
            _addressActionHandler.Edit.Enabled = false;
            _addressActionHandler.Delete.Enabled = false;

            _addressTypes = addressTypes;
        }

        public IList<AddressDetail> Subject
        {
            get { return _addressList; }
            set { _addressList = value; }
        }

        public override void Start()
        {
            if (_addressList != null)
            {
                foreach (AddressDetail address in _addressList)
                {
                    _addresses.Items.Add(address);
                }
            }
            base.Start();
        }

        #region Presentation Model

        public ITable Addresses
        {
            get { return _addresses; }
        }

        public ActionModelNode AddressListActionModel
        {
            get { return _addressActionHandler; }
        }

        public ISelection SelectedAddress
        {
            get { return new Selection(_currentAddressSelection); }
            set
            {
                _currentAddressSelection = (AddressDetail)value.Item;
                AddressSelectionChanged();
            }
        }

        public void AddAddress()
        {
            try
            {
                AddressDetail address = new AddressDetail();
                address.Province = CollectionUtils.FirstElement<string>(AddressEditorComponentSettings.Default.ProvinceChoices);
                address.Country = CollectionUtils.FirstElement<string>(AddressEditorComponentSettings.Default.CountryChoices);
                address.Type = _addressTypes[0];

                AddressEditorComponent editor = new AddressEditorComponent(address, _addressTypes);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddAddress);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    _addresses.Items.Add(address);
                    _addressList.Add(address);
                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // failed to obtain country/province choices, or failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void UpdateSelectedAddress()
        {
            try
            {
                // can occur if user double clicks while holding control
                if (_currentAddressSelection == null) return;

                AddressDetail address = (AddressDetail)_currentAddressSelection.Clone();
                AddressEditorComponent editor = new AddressEditorComponent(address, _addressTypes);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateAddress);
                if (exitCode == ApplicationComponentExitCode.Accepted)
                {
                    // delete and re-insert to ensure that TableView updates correctly
                    AddressDetail toBeRemoved = _currentAddressSelection;
                    _addresses.Items.Remove(toBeRemoved);
                    _addressList.Remove(toBeRemoved);

                    _addresses.Items.Add(address);
                    _addressList.Add(address);

                    this.Modified = true;
                }
            }
            catch (Exception e)
            {
                // failed to launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void DeleteSelectedAddress()
        {
            if (this.Host.ShowMessageBox( SR.MessageConfirmDeleteSelectedAddress, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary Address otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Address being removed from the PatientProfile
                AddressDetail toBeRemoved = _currentAddressSelection;
                _addresses.Items.Remove(toBeRemoved);
                _addressList.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

        private void AddressSelectionChanged()
        {
            _addressActionHandler.Edit.Enabled =
                _addressActionHandler.Delete.Enabled = (_currentAddressSelection != null);
        }
    }
}
