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
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ContactPersonEditorComponent"/>
    /// </summary>
    public partial class ContactPersonEditorComponentControl : ApplicationComponentUserControl
    {
        private ContactPersonEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ContactPersonEditorComponentControl(ContactPersonEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _type.DataSource = _component.TypeChoices;
            _type.DataBindings.Add("Value", _component, "Type", true, DataSourceUpdateMode.OnPropertyChanged);

            _relationship.DataSource = _component.RelationshipChoices;
            _relationship.DataBindings.Add("Value", _component, "Relationship", true, DataSourceUpdateMode.OnPropertyChanged);

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _address.DataBindings.Add("Value", _component, "Address", true, DataSourceUpdateMode.OnPropertyChanged);
            _homePhone.DataBindings.Add("Value", _component, "HomePhoneNumber", true, DataSourceUpdateMode.OnPropertyChanged);
            _businessPhone.DataBindings.Add("Value", _component, "BusinessPhoneNumber", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void ContactPersonEditorComponentControl_Load(object sender, EventArgs e)
        {
            _homePhone.Mask = _component.PhoneNumberMask;
            _businessPhone.Mask = _component.PhoneNumberMask;
        }
    }
}
