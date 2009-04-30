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
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop.Validation;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Extension point for views onto <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ContactPersonEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ContactPersonEditorComponent class
    /// </summary>
    [AssociateView(typeof(ContactPersonEditorComponentViewExtensionPoint))]
    public class ContactPersonEditorComponent : ApplicationComponent
    {
        private ContactPersonDetail _contactPerson;
        private IList<EnumValueInfo> _contactTypeChoices;
        private IList<EnumValueInfo> _contactRelationshipChoices;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonEditorComponent(ContactPersonDetail contactPerson, IList<EnumValueInfo> contactTypeChoices, IList<EnumValueInfo> contactRelationshipChoices)
        {
            _contactPerson = contactPerson;
            _contactTypeChoices = contactTypeChoices;
            _contactRelationshipChoices = contactRelationshipChoices;
        }

        public override void Start()
        {
            base.Start();

            if (_contactPerson.Relationship == null)
                _contactPerson.Relationship = _contactRelationshipChoices[0];
 
            if (_contactPerson.Type == null)
                _contactPerson.Type = _contactTypeChoices[0];
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Presentation Model

        [ValidateNotNull]
        public string Name
        {
            get { return _contactPerson.Name; }
            set
            {
                _contactPerson.Name = value;
                this.Modified = true;
            }
        }

        public string Address
        {
            get { return _contactPerson.Address; }
            set
            {
                _contactPerson.Address = value;
                this.Modified = true;
            }
        }

        public string HomePhoneNumber
        {
            get { return _contactPerson.HomePhoneNumber; }
            set
            {
                _contactPerson.HomePhoneNumber = value;
                this.Modified = true;
            }
        }

        public string BusinessPhoneNumber
        {
            get { return _contactPerson.BusinessPhoneNumber; }
            set
            {
                _contactPerson.BusinessPhoneNumber = value;
                this.Modified = true;
            }
        }

        public string PhoneNumberMask
        {
            get { return TextFieldMasks.TelphoneNumberFullMask; }
        }

        [ValidateNotNull]
        public string Type
        {
            get { return _contactPerson.Type.Value; }
            set
            {
                _contactPerson.Type = EnumValueUtils.MapDisplayValue(_contactTypeChoices, value);
                this.Modified = true;
            }
        }

        public List<string> TypeChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_contactTypeChoices); }
        }

        [ValidateNotNull]
        public string Relationship
        {
            get { return _contactPerson.Relationship.Value; }
            set
            {
                _contactPerson.Relationship = EnumValueUtils.MapDisplayValue(_contactRelationshipChoices, value);
                this.Modified = true;
            }
        }

        public List<string> RelationshipChoices
        {
            get { return EnumValueUtils.GetDisplayValues(_contactRelationshipChoices); }
        }

        public void Accept()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
            }
            else
            {
                this.ExitCode = ApplicationComponentExitCode.Accepted;
                Host.Exit();
            }
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

        #endregion
    }
}
