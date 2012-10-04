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
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RecordGeneratorLaunchPadComponent"/>
    /// </summary>
    public partial class RecordGeneratorLaunchPadComponentControl : CustomUserControl
    {
        private RecordGeneratorLaunchPadComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordGeneratorLaunchPadComponentControl(RecordGeneratorLaunchPadComponent component)
        {
            InitializeComponent();

            _component = component;

            _settingsTable.Table = _component.Settings;
            _measureSelectionTable.Table = _component.MeasureSelections;
            _measureSelectionTable.SelectionChanged += new EventHandler(OnMeasureSelectionChanged);
            _measureSettingsTable.Table = _component.MeasureSettings;
            _generatorCombo.DataSource = _component.GeneratorChoices;
            _generatorCombo.DataBindings.Add("Value", _component, "SelectedGeneratorName", true, DataSourceUpdateMode.OnPropertyChanged);
            _launchButton.DataBindings.Add("Enabled", _component, "LaunchEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _numberToBeGenerated.DataBindings.Add("Value", _component, "NumberOfEntitiesToBeGenerated", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _launchButton_Click(object sender, EventArgs e)
        {
            _component.Launch();
        }

        private void OnMeasureSelectionChanged(object sender, EventArgs e)
        {
            _component.SetMeasureSelection(_measureSelectionTable.Selection);
        }
    }
}
