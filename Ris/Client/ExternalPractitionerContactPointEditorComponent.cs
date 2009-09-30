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
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// ExternalPractitionerContactPointEditorComponent class
    /// </summary>
    public class ExternalPractitionerContactPointEditorComponent : NavigatorComponentContainer
    {
        private readonly ExternalPractitionerContactPointDetail _contactPointDetail;

        private ExternalPractitionerContactPointDetailsEditorComponent _detailsEditor;
        private AddressesSummaryComponent _addressesSummary;
        private PhoneNumbersSummaryComponent _phoneNumbersSummary;

        private readonly List<EnumValueInfo> _addressTypeChoices;
        private readonly List<EnumValueInfo> _phoneTypeChoices;
        private readonly List<EnumValueInfo> _resultCommunicationModeChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerContactPointEditorComponent(ExternalPractitionerContactPointDetail contactPoint, List<EnumValueInfo> addressTypeChoices, List<EnumValueInfo> phoneTypeChoices, List<EnumValueInfo> resultCommunicationModeChoices)
        {
            _contactPointDetail = contactPoint;
            _addressTypeChoices = addressTypeChoices;
            _phoneTypeChoices = phoneTypeChoices;
            _resultCommunicationModeChoices = resultCommunicationModeChoices;
        }

        public override void Start()
        {
            string rootPath = "Contact Point";
            this.Pages.Add(new NavigatorPage(rootPath, _detailsEditor = new ExternalPractitionerContactPointDetailsEditorComponent(_contactPointDetail, _resultCommunicationModeChoices)));
            this.Pages.Add(new NavigatorPage(rootPath + "/Addresses", _addressesSummary = new AddressesSummaryComponent(_addressTypeChoices)));
            this.Pages.Add(new NavigatorPage(rootPath + "/Phone Numbers", _phoneNumbersSummary = new PhoneNumbersSummaryComponent(_phoneTypeChoices)));

        	_addressesSummary.SetModifiedOnListChange = true;
        	_phoneNumbersSummary.SetModifiedOnListChange = true;

            this.ValidationStrategy = new AllComponentsValidationStrategy();

            _addressesSummary.Subject = _contactPointDetail.Addresses;
            _phoneNumbersSummary.Subject = _contactPointDetail.TelephoneNumbers;

            base.Start();
        }
    }
}
