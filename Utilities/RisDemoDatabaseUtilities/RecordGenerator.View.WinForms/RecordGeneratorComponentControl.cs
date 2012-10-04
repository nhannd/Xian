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
using System.IO;

using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator;
using ClearCanvas.Common;

namespace ClearCanvas.Utilities.RisDemoDatabaseUtilities.RecordGenerator.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="RecordGeneratorComponent"/>
    /// </summary>
    public partial class RecordGeneratorComponentControl : UserControl
    {
        private RecordGeneratorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public RecordGeneratorComponentControl(RecordGeneratorComponent component)
        {
            Platform.CheckForNullReference(component, "component");
            InitializeComponent();

            _component = component;

            _statTableView.Table = _component.StatisticTable;
            _measureTableView.Table = _component.MeasureTable;

            _start.DataBindings.Add("Enabled", _component, "StartEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _stop.DataBindings.Add("Enabled", _component, "StopEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _statExport.DataBindings.Add("Enabled", _component, "StatsExportEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _measureExport.DataBindings.Add("Enabled", _component, "MeasureExportEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _start_Click(object sender, EventArgs e)
        {            
            _component.BeginGeneration();
        }

        private void _stop_Click(object sender, EventArgs e)
        {
            _component.StopGeneration();
        }

        private void _statExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog statsSave = new SaveFileDialog();
            statsSave.Title = "Save Statistics";
            statsSave.Filter = "Comma Separated Values (*.csv)|*.csv|All files (*.*)|*.*";
            statsSave.FilterIndex = 1;
            statsSave.RestoreDirectory = true;

            if (statsSave.ShowDialog() == DialogResult.OK)
            {
                _component.ExportStats(statsSave.OpenFile());
            }
        }

        private void _measureExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog measureSave = new SaveFileDialog();
            measureSave.Title = "Save Measures";
            measureSave.Filter = "Comma Separated Values (*.csv)|*.csv|All files (*.*)|*.*";
            measureSave.FilterIndex = 1;
            measureSave.RestoreDirectory = true;

            if (measureSave.ShowDialog() == DialogResult.OK)
            {
                _component.ExportMeasures(measureSave.OpenFile());
            }
        }
    }
}
