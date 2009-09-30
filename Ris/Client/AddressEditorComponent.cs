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
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using System.Collections;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint]
    public class AddressesEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(AddressesEditorComponentViewExtensionPoint))]
    public class AddressEditorComponent : ApplicationComponent
    {
        private AddressDetail _address;
        private readonly List<EnumValueInfo> _addressTypes;
        private readonly bool _addressTypeEnabled;


        public AddressEditorComponent(AddressDetail address, List<EnumValueInfo> addressTypes)
        {
            _address = address;
            _addressTypes = addressTypes;
            _addressTypeEnabled = addressTypes.Count > 1;
        }

        /// <summary>
        /// Sets the subject upon which the editor acts
        /// Not for use by the view
        /// </summary>
        public AddressDetail Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public override void Start()
        {
            base.Start();
        }

        [ValidateNotNull]
        public string Street
        {
            get { return _address.Street; }
            set
            {
                _address.Street = value;
                this.Modified = true;
            }
        }

        public string Unit
        {
            get { return _address.Unit; }
            set
            {
                _address.Unit = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string City
        {
            get { return _address.City; }
            set
            {
                _address.City = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Province
        {
            get { return _address.Province; }
            set
            {
                _address.Province = value;
                this.Modified = true;
            }
        }

        public ICollection<string> ProvinceChoices
        {
            get { return AddressEditorComponentSettings.Default.ProvinceChoices; }
        }

        public string Country
        {
            get { return _address.Country; }
            set
            {
                _address.Country = value;
                this.Modified = true;
            }
        }

        public ICollection<string> CountryChoices
        {
            get { return AddressEditorComponentSettings.Default.CountryChoices; }
        }

        public string PostalCode
        {
            get { return _address.PostalCode; }
            set
            {
                _address.PostalCode = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidFrom
        {
            get { return _address.ValidRangeFrom; }
            set {
                _address.ValidRangeFrom = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _address.ValidRangeUntil; }
            set {
                _address.ValidRangeUntil = value;
                this.Modified = true;
            }
        }

        public bool AddressTypeEnabled
        {
            get { return _addressTypeEnabled; }
        }

        public EnumValueInfo AddressType
        {
            get { return _address.Type; }
            set
            {
            	_address.Type = value;
                this.Modified = true;
            }
        }

        public IList AddressTypeChoices
        {
            get { return _addressTypes; }
        }

        public void Accept()
        {
			if (this.HasValidationErrors)
			{
				this.ShowValidation(true);
				return;
			}

			this.ExitCode = ApplicationComponentExitCode.Accepted;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

    }
}
