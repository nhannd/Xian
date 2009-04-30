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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common;
using System.Collections;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ExternalPractitionerDetailsEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ExternalPractitionerDetailsEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ExternalPractitionerDetailsEditorComponent class
    /// </summary>
    [AssociateView(typeof(ExternalPractitionerDetailsEditorComponentViewExtensionPoint))]
    public class ExternalPractitionerDetailsEditorComponent : ApplicationComponent
    {
        private ExternalPractitionerDetail _practitionerDetail;
        private bool _isNew;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerDetailsEditorComponent(bool isNew)
        {
            _practitionerDetail = new ExternalPractitionerDetail();
            _isNew = isNew;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public ExternalPractitionerDetail ExternalPractitionerDetail
        {
            get { return _practitionerDetail; }
            set 
            { 
                _practitionerDetail = value;
            }
        }

        #region Presentation Model

        [ValidateNotNull]
        public string FamilyName
        {
            get { return _practitionerDetail.Name.FamilyName; }
            set 
            {
                _practitionerDetail.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        [ValidateNotNull]
        public string GivenName
        {
            get { return _practitionerDetail.Name.GivenName; }
            set
            {
                _practitionerDetail.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _practitionerDetail.Name.MiddleName; }
            set
            {
                _practitionerDetail.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Prefix
        {
            get { return _practitionerDetail.Name.Prefix; }
            set
            {
                _practitionerDetail.Name.Prefix = value;
                this.Modified = true;
            }
        }

        public string Suffix
        {
            get { return _practitionerDetail.Name.Suffix; }
            set
            {
                _practitionerDetail.Name.Suffix = value;
                this.Modified = true;
            }
        }

        public string Degree
        {
            get { return _practitionerDetail.Name.Degree; }
            set
            {
                _practitionerDetail.Name.Degree = value;
                this.Modified = true;
            }
        }

        public string LicenseNumber
        {
            get { return _practitionerDetail.LicenseNumber; }
            set
            {
                _practitionerDetail.LicenseNumber = value;
                this.Modified = true;
            }
        }

        public string BillingNumber
        {
            get { return _practitionerDetail.BillingNumber; }
            set
            {
                _practitionerDetail.BillingNumber = value;
                this.Modified = true;
            }
        }

        #endregion
    }
}
