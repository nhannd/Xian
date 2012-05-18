#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class GenerateDatabase : Form
    {
        private readonly SopGeneratorHospital _hospital1 = new SopGeneratorHospital()
                                                      {
                                                          Accession = 1000000,
                                                          Name = "MSH",
                                                          RequestedProcedureStepId = 1000,
                                                          ScheduledProcedureStepId = 2000
                                                      };

        private readonly SopGeneratorHospital _hospital2 = new SopGeneratorHospital()
                                                      {
                                                          Accession = 2000000,
                                                          Name = "TGH",
                                                          RequestedProcedureStepId = 10000,
                                                          ScheduledProcedureStepId = 20000
                                                      };

        private readonly SopGeneratorHospital _hospital3 = new SopGeneratorHospital()
                                                      {
                                                          Accession = 3000000,
                                                          Name = "TWH",
                                                          RequestedProcedureStepId = 80000,
                                                          ScheduledProcedureStepId = 90000
                                                      };

        private readonly SopGeneratorHospital _hospital4 = new SopGeneratorHospital()
                                                               {
                                                                   Accession = 4000000,
                                                                   Name = "WCH",
                                                                   RequestedProcedureStepId = 50000,
                                                                   ScheduledProcedureStepId = 60000
                                                               };
        private ImageServerDbGenerator _generator = null;

        public GenerateDatabase()
        {
            InitializeComponent();
            _buttonCancel.Enabled = false;
            bool first = true;
            foreach (ServerPartition partition in ServerPartitionMonitor.Instance)
            {
                _comboBoxServerPartition.Items.Add(partition);
                if (first)
                {
                    first = false;
                    _comboBoxServerPartition.SelectedItem = partition;
                }
            }
            _comboBoxServerPartition.ValueMember = "Description";

            // initialize
            _checkBoxImageServerDatabase_CheckedChanged(null, null);
        }

        void ProgressUpdated(object sender, BackgroundTaskProgressEventArgs e)
        {
            _progressBar.Text = e.Progress.Message;
            _progressBar.Value = e.Progress.Percent > 100 ? 100 : e.Progress.Percent;
            _progressBar.Update();

            if (e.Progress.Percent >= 100)
            {
                _buttonCancel.Enabled = false;
                _buttonGenerate.Enabled = true;
            }
        }

        private void _buttonGenerate_Click(object sender, EventArgs e)
        {
            int totalStudies;
            int studiesPerDay;

            if (!int.TryParse(_textBoxTotalStudies.Text, out totalStudies))
                totalStudies = 50000;
            if (!int.TryParse(_textBoxStudiesPerDay.Text, out studiesPerDay))
                studiesPerDay = 1800;
            
            if (_checkBoxImageServerDatabase.Checked)
            {
                if (_comboBoxServerPartition.SelectedItem == null)
                    return;

                ServerPartition selectedPartition = _comboBoxServerPartition.SelectedItem as ServerPartition;
                if (selectedPartition == null)
                    return;
                
                _generator = new ImageServerDbGenerator(selectedPartition, _dateTimePickerStart.Value, totalStudies,
                                                        studiesPerDay,
                                                        (int) _numericUpDownPercentWeekend.Value);
            }
            else
            {
                _generator = new ImageServerDbGenerator(_textBoxRemoteAETitle.Text, _textBoxHost.Text,
                                                        int.Parse(_textBoxPort.Text), _dateTimePickerStart.Value,
                                                        totalStudies,
                                                        studiesPerDay,
                                                        (int) _numericUpDownPercentWeekend.Value);

            }
            _progressBar.Maximum = 0;
            _progressBar.Maximum = 100;
            _generator.RegisterProgressUpated(ProgressUpdated);
            
            if (_checkBoxMR.Checked)
            {
                _generator.AddSopGenerator(new MrSopGenerator(_hospital1));
                _generator.AddSopGenerator(new MrSopGenerator(_hospital2));
                _generator.AddSopGenerator(new MrSopGenerator(_hospital3));
            }
            if (_checkBoxCT.Checked)
            {
                _generator.AddSopGenerator(new CtSopGenerator(_hospital1));
                _generator.AddSopGenerator(new CtSopGenerator(_hospital4));
                _generator.AddSopGenerator(new CtSopGenerator(_hospital3));
            }
            if (_checkBoxCR.Checked)
            {
                _generator.AddSopGenerator(new CrSopGenerator(_hospital1));
                _generator.AddSopGenerator(new CrSopGenerator(_hospital3));
            }
            if (_checkBoxDX.Checked)
            {
                _generator.AddSopGenerator(new DxSopGenerator(_hospital1));
                _generator.AddSopGenerator(new DxSopGenerator(_hospital4));
            }
            if (_checkBoxRF.Checked)
            {
                _generator.AddSopGenerator(new RfSopGenerator(_hospital2));
            }
            if (_checkBoxXA.Checked)
            {
                _generator.AddSopGenerator(new XaSopGenerator(_hospital1));
            }
            if (_checkBoxMG.Checked)
            {
                _generator.AddSopGenerator(new MgSopGenerator(_hospital1));
                _generator.AddSopGenerator(new MgSopGenerator(_hospital2));
            }
            if (_checkBoxUS.Checked)
            {
                _generator.AddSopGenerator(new UsSopGenerator(_hospital1));
                _generator.AddSopGenerator(new UsSopGenerator(_hospital4));
                _generator.AddSopGenerator(new UsSopGenerator(_hospital3));
            }



            _buttonCancel.Enabled = true;
            _buttonGenerate.Enabled = false;
            
            _generator.Start();
        }

        private void _buttonCancel_Click(object sender, EventArgs e)
        {
            _buttonCancel.Enabled = false;
            _buttonGenerate.Enabled = true;
            if (_generator != null)
            {
                _generator.Cancel();
                _generator = null;
            }
        }

        private void _checkBoxImageServerDatabase_CheckedChanged(object sender, EventArgs e)
        {
            if (_checkBoxImageServerDatabase.Checked)
            {
                _comboBoxServerPartition.Enabled = true;
                _textBoxHost.Enabled = false;
                _textBoxPort.Enabled = false;
                _textBoxRemoteAETitle.Enabled = false;
            }
            else
            {
                _comboBoxServerPartition.Enabled = false;
                _textBoxHost.Enabled = true;
                _textBoxPort.Enabled = true;
                _textBoxRemoteAETitle.Enabled = true;
            }
        }
    }
}
