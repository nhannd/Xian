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
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

using Iesi.Collections;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint()]
    public class PatientEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(PatientEditorComponentViewExtensionPoint))]
    public class PatientProfileDetailsEditorComponent : ApplicationComponent
    {
        private PatientProfileDetail _profile;
        private List<EnumValueInfo> _sexChoices;

        private string[] _dummyHealthcardAssigningAuthorityChoices = new string[] { "", "Ontario", "Alberta", "British Columbia", "Manitoba", "New Brunswick", "Newfoundland", "Nova Scotia", "PEI", "Quebec", "Saskatchewan" };
        private string[] _dummyMrnAssigningAuthorityChoices = new string[] { "", "UHN", "MSH", "SiteA", "SiteB", "SiteC", "SiteD", "SiteE", "SiteF" };

        public PatientProfileDetailsEditorComponent(List<EnumValueInfo> sexChoices)
        {
            this.Validation.Add(PatientProfileDetailsEditorComponentSettings.Default.ValidationRules);

            _sexChoices = sexChoices;
        }

        /// <summary>
        /// Gets or sets the subject (e.g PatientProfileDetail) that this editor operates on.  This property
        /// should never be used by the view.
        /// </summary>
        public PatientProfileDetail Subject
        {
            get { return _profile; }
            set { _profile = value; }
        }

        public override void Start()
        {
            base.Start();
        }

        #region Presentation Model

        public string MrnID
        {
            get { return _profile.Mrn.Id; }
            set
            {
                _profile.Mrn.Id = value;
                this.Modified = true;
            }
        }

        public string MrnSite
        {
            get { return _profile.Mrn.AssigningAuthority; }
            set
            {
                _profile.Mrn.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public string FamilyName
        {
            get { return _profile.Name.FamilyName; }
            set { 
                _profile.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _profile.Name.GivenName; }
            set { 
                _profile.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _profile.Name.MiddleName; }
            set { 
                _profile.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Sex
        {
            get { return _profile.Sex.Value; }
            set
            {
                _profile.Sex = EnumValueUtils.MapDisplayValue(_sexChoices, value);
                this.Modified = true;
            }
        }

        public List<string> SexChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_sexChoices); }
        }

        public DateTime DateOfBirth
        {
            get { return _profile.DateOfBirth.Value; }
            set
            { 
                _profile.DateOfBirth = value.Date;
                this.Modified = true;
            }
        }

        public bool DeathIndicator
        {
            get { return _profile.DeathIndicator; }
            set
            { 
                _profile.DeathIndicator = value;
                this.Modified = true;
            }
        }

        public DateTime? TimeOfDeath
        {
            get { return _profile.TimeOfDeath; }
            set
            {
                _profile.TimeOfDeath = value;
                _profile.DeathIndicator = (_profile.TimeOfDeath == null ? false : true);
                this.Modified = true;
            }
        }

        public string[] MrnAssigningAuthorityChoices
        {
            get { return _dummyMrnAssigningAuthorityChoices;  }
        }

        public string HealthcardID
        {
            get { return _profile.Healthcard.Id; }
            set
            {
                _profile.Healthcard.Id = value;
                this.Modified = true;
            }
        }

        public string HealtcardMask
        {
            get { return TextFieldMasks.HealthcardNumberMask; }
        }

        public string HealthcardProvince
        {
            get { return _profile.Healthcard.AssigningAuthority; }
            set
            {
                _profile.Healthcard.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public string[] HealthcardAssigningAuthorityChoices
        {
            get { return _dummyHealthcardAssigningAuthorityChoices;  }
        }

        public string HealthcardVersionCode
        {
            get { return _profile.Healthcard.VersionCode; }
            set
            {
                _profile.Healthcard.VersionCode = value;
                this.Modified = true;
            }
        }

        public string HealthcardVersionCodeMask
        {
            get { return TextFieldMasks.HealthcardVersionCodeMask; }
        }

        public DateTime? HealthcardExpiryDate
        {
            get { return _profile.Healthcard.ExpiryDate; }
            set
            {
                _profile.Healthcard.ExpiryDate = value;
                this.Modified = true;
            }
        }

        #endregion
    }
}
