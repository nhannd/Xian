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
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using System.Collections;

namespace ClearCanvas.Ris.Client
{
    [ExtensionPoint()]
    public class PhoneNumbersEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    public class PhoneNumberEditorComponent : ApplicationComponent
    {
        private readonly TelephoneDetail _phoneNumber;
        private readonly List<EnumValueInfo> _phoneTypeChoices;
        private readonly bool _phoneTypeEnabled;

        public PhoneNumberEditorComponent(TelephoneDetail phoneNumber, List<EnumValueInfo> phoneTypeChoices)
        {
            _phoneNumber = phoneNumber;
            _phoneTypeChoices = phoneTypeChoices;
            _phoneTypeEnabled = phoneTypeChoices.Count > 1;
        }

        public override void Start()
        {
            base.Start();
        }

        public string PhoneNumberMask
        {
            get { return TextFieldMasks.TelephoneNumberLocalMask; }
        }

        public string CountryCode
        {
            get { return _phoneNumber.CountryCode; }
            set
            {
                _phoneNumber.CountryCode = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string AreaCode
        {
            get { return _phoneNumber.AreaCode; }
            set
            {
               _phoneNumber.AreaCode = value;
               this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string Number
        {
            get { return _phoneNumber.Number; }
            set
            {
                _phoneNumber.Number = value;
                this.Modified = true;
            }
        }

        public string Extension
        {
            get { return _phoneNumber.Extension; }
            set
            {
                _phoneNumber.Extension = value;
                this.Modified = true;
            }
        }

        public bool PhoneTypeEnabled
        {
            get { return _phoneTypeEnabled; }
        }

        [ValidateNotNull]
        public EnumValueInfo PhoneType
        {
            get { return _phoneNumber.Type; }
            set
            {
                _phoneNumber.Type = value;
                this.Modified = true;
            }
        }

        public IList PhoneTypeChoices
        {
            get { return _phoneTypeChoices; }
        }

        public DateTime? ValidFrom
        {
            get { return _phoneNumber.ValidRangeFrom; }
            set
            {
                _phoneNumber.ValidRangeFrom = value;
                this.Modified = true;
            }
        }

        public DateTime? ValidUntil
        {
            get { return _phoneNumber.ValidRangeUntil; }
            set
            {
                _phoneNumber.ValidRangeUntil = value;
                this.Modified = true;
            }
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
