#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="PatientProfileAdditionalInfoEditorComponent"/>
    /// </summary>
    public partial class PatientProfileAdditionalInfoEditorComponentControl : CustomUserControl
    {
        private PatientProfileAdditionalInfoEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public PatientProfileAdditionalInfoEditorComponentControl(PatientProfileAdditionalInfoEditorComponent component)
        {
            InitializeComponent();

            _component = component;

            _primaryLanguage.DataSource = _component.LanguageChoices;
            _primaryLanguage.DataBindings.Add("Value", _component, "Language", true, DataSourceUpdateMode.OnPropertyChanged);

            _religion.DataSource = _component.ReligionChoices;
            _religion.DataBindings.Add("Value", _component, "Religion", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}