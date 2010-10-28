#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
    public partial class PhoneNumberEditorControl : ApplicationComponentUserControl
    {
        private PhoneNumberEditorComponent _component;

        public PhoneNumberEditorControl(PhoneNumberEditorComponent component)
			: base(component)
        {
            InitializeComponent();
            _component = component;

            _phoneType.DataSource = _component.PhoneTypeChoices;
            _phoneType.DataBindings.Add("Value", _component, "PhoneType", true, DataSourceUpdateMode.OnPropertyChanged);
            _phoneType.DataBindings.Add("Enabled", _component, "PhoneTypeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _countryCode.DataBindings.Add("Value", _component, "CountryCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _areaCode.DataBindings.Add("Value", _component, "AreaCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _number.DataBindings.Add("Value", _component, "Number", true, DataSourceUpdateMode.OnPropertyChanged);
            _extension.DataBindings.Add("Value", _component, "Extension", true, DataSourceUpdateMode.OnPropertyChanged);

            _validFrom.DataBindings.Add("Value", _component, "ValidFrom", true, DataSourceUpdateMode.OnPropertyChanged);
            _validUntil.DataBindings.Add("Value", _component, "ValidUntil", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void PhoneNumberEditorControl_Load(object sender, EventArgs e)
        {
            _number.Mask = _component.PhoneNumberMask;
        }
    }
}
