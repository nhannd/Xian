#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Common.Utilities;

using System.IO;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Client;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="PatientPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class PatientPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ExtensionPoint]
    public class PatientPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IPatientPreviewToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// PatientPreviewComponent class
    /// </summary>
    [AssociateView(typeof(PatientPreviewComponentViewExtensionPoint))]
    public class PatientProfilePreviewComponent : ApplicationComponent
    {
        class PatientPreviewToolContext : ToolContext, IPatientPreviewToolContext
        {
            private PatientProfilePreviewComponent _component;
            public PatientPreviewToolContext(PatientProfilePreviewComponent component)
            {
                _component = component;
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }


        private EntityRef<PatientProfile> _patientProfileRef;
        private PatientProfile _patientProfile;
        private bool _showHeader;
        private bool _showReconciliationAlert;

        private IAdtService _adtService;
        private AddressTable _addresses;
        private TelephoneNumberTable _phoneNumbers;
        private SexEnumTable _sexChoices;

        private ToolSet _toolSet;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfilePreviewComponent()
            :this(true, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfilePreviewComponent(bool showHeader, bool showReconciliationAlert)
        {
            _showHeader = showHeader;
            _showReconciliationAlert = showReconciliationAlert;
        }

        public EntityRef<PatientProfile> PatientProfileRef
        {
            get { return _patientProfileRef; }
            set
            {
                _patientProfileRef = value;
                if (this.IsStarted)
                {
                    UpdateDisplay();
                }
            }
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();
            _sexChoices = _adtService.GetSexEnumTable();

            _addresses = new AddressTable();
            _phoneNumbers = new TelephoneNumberTable();
            _toolSet = new ToolSet(new PatientPreviewToolExtensionPoint(), new PatientPreviewToolContext(this));

            UpdateDisplay();
            
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        private void UpdateDisplay()
        {
            _addresses.Items.Clear();
            _phoneNumbers.Items.Clear();

            if (_patientProfileRef != null)
            {
                _patientProfile = _adtService.LoadPatientProfile(_patientProfileRef, true);

                Address mostRecentExpiredHomeAddress = null;
                foreach (Address address in _patientProfile.Addresses)
                {
                    if (address.IsCurrent)
                    {
                        _addresses.Items.Add(address);
                    }
                    else
                    {
                        if (address.Type == AddressType.R)
                        {
                            if (mostRecentExpiredHomeAddress == null || address.ValidRange.Until > mostRecentExpiredHomeAddress.ValidRange.Until)
                            {
                                mostRecentExpiredHomeAddress = address;
                            }
                        }
                    }
                }
                if (mostRecentExpiredHomeAddress != null) _addresses.Items.Add(mostRecentExpiredHomeAddress);
                if (_patientProfile.CurrentHomeAddress != null) _addresses.Items.Remove(_patientProfile.CurrentHomeAddress);
                _addresses.Sort(new TableSortParams(_addresses.Columns[0], false));

                TelephoneNumber mostRecentExpiredHomePhone = null;
                foreach (TelephoneNumber phoneNumber in _patientProfile.TelephoneNumbers)
                {
                    if (phoneNumber.IsCurrent)
                    {
                        _phoneNumbers.Items.Add(phoneNumber);
                    }
                    else
                    {
                        if (phoneNumber.Use == TelephoneUse.PRN)
                        {
                            if (mostRecentExpiredHomePhone == null || phoneNumber.ValidRange.Until > mostRecentExpiredHomePhone.ValidRange.Until)
                            {
                                mostRecentExpiredHomePhone = phoneNumber;
                            }
                        }
                    }
                }
                if (mostRecentExpiredHomePhone != null) _phoneNumbers.Items.Add(mostRecentExpiredHomePhone);
                if (_patientProfile.CurrentHomePhone != null) _phoneNumbers.Items.Remove(_patientProfile.CurrentHomePhone);
                _phoneNumbers.Sort(new TableSortParams(_phoneNumbers.Columns[0], false));
            }

            NotifyAllPropertiesChanged();
        }

        #region Presentation Model

        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        public bool ShowReconciliationAlert
        {
            get
            {
                //EntityRef<PatientProfile> patientProfileRef = new EntityRef<PatientProfile>(_
                return _showReconciliationAlert &&
                    _adtService.FindPatientReconciliationMatches(_patientProfileRef).Count > 0;
            }
            set { _showReconciliationAlert = value; }
        }

        public string Name
        {
            get { return Format.Custom(_patientProfile.Name); }
        }

        public string DateOfBirth
        {
            get { return ClearCanvas.Desktop.Format.Date(_patientProfile.DateOfBirth); }
        }

        public string Mrn
        {
            get { return Format.Custom(_patientProfile.Mrn); }
        }

        public string Healthcard
        {
            get { return Format.Custom(_patientProfile.Healthcard); }
        }

        public string Sex
        {
            get { return _sexChoices[_patientProfile.Sex].Value; }
        }

        public string CurrentHomeAddress
        {
            get
            {
                Address address = _patientProfile.CurrentHomeAddress;
                return (address == null) ? SR.TextUnknownValue : Format.Custom(address);
            }
        }

        public string CurrentHomePhone
        {
            get
            {
                TelephoneNumber phone = _patientProfile.CurrentHomePhone;
                return (phone == null) ? SR.TextUnknownValue : Format.Custom(phone);
            }
        }

        public ITable Addresses
        {
            get { return _addresses; }
        }

        public int MoreAddressesCount
        {
            get 
            { 
                return (_patientProfile.CurrentHomeAddress != null) 
                    ? (_patientProfile.Addresses.Count - 1) - _addresses.Items.Count
                    : _patientProfile.Addresses.Count - _addresses.Items.Count; 
            }
        }

        public ITable PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        public int MorePhoneNumbersCount
        {
            get 
            {
                return (_patientProfile.CurrentHomePhone != null)
                    ? (_patientProfile.TelephoneNumbers.Count - 1) - _phoneNumbers.Items.Count
                    : _patientProfile.TelephoneNumbers.Count - _phoneNumbers.Items.Count;
            }
        }

        public ActionModelNode MenuModel
        {
            get
            {
                return ActionModelRoot.CreateModel(this.GetType().FullName, "patientpreview-menu", _toolSet.Actions);
            }
        }

        #endregion
    }
}
