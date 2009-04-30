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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.ImageServer.TestApp
{

   
    public partial class TestSendImagesForm : Form
    {
        private Dictionary<string, string> _seriesMap;
        private Dictionary<string, List<string>> _seriesToFilesMap;
        private List<string> _lastNames = new List<string>();
        private List<string> _givenNames = new List<string>();
        private List<string> _seriesDesc = new List<string>();
        public TestSendImagesForm()
        {
            InitializeComponent();

            StreamReader re = File.OpenText("LastNames.txt");
            string input = null;
            while ((input = re.ReadLine()) != null)
            {
                _lastNames.Add(input.Trim());
            }
            re.Close();

            re = File.OpenText("GivenNames.txt");
            while ((input = re.ReadLine()) != null)
            {
                _givenNames.Add(input.Trim());
            }
            re.Close();

            re = File.OpenText("SeriesDescriptions.txt");
            while ((input = re.ReadLine()) != null)
            {
                _seriesDesc.Add(input.Trim());
            }
            re.Close();

            InitNewPatient(); 
            InitNewStudy();

        }

        private void SendRandom_Click(object sender, EventArgs e)
        {
            Random ran = new Random();
            String seriesDescription = _seriesDesc[ran.Next(_seriesDesc.Count)];

            if (InputBox("Series Description", "Series Description:", ref seriesDescription) == DialogResult.OK)
            {
                _seriesMap = new Dictionary<string, string>();

                do
                {
                    using (StorageScu scu = new StorageScu(LocalAE.Text, ServerAE.Text, ServerHost.Text, int.Parse(ServerPort.Text)))
                    {
                        string[] seriesUids = new string[_seriesToFilesMap.Count];
                        _seriesToFilesMap.Keys.CopyTo(seriesUids, 0);
                        String seriesToUse = seriesUids[ran.Next(_seriesToFilesMap.Count)];
                        List<string> files = _seriesToFilesMap[seriesToUse];
                        foreach (string path in files)
                        {
                            DicomFile file = new DicomFile(path);
                            file.Load();

                            RandomizeFile(file, seriesDescription);
                            scu.AddStorageInstance(new StorageInstance(file));
                            if (ran.Next() % 20 == 0)
                                break; // don't use all images
                        }
                        scu.Send();
                        scu.Join();
                    }
                } while (ran.Next() % 3 != 0);

            }
        }

        private void RandomizeFile(DicomFile file, String seriesDescription)
        {
            string seriesUid = file.DataSet[DicomTags.SeriesInstanceUid].ToString();
            if (!_seriesMap.ContainsKey(seriesUid))
            {
                string newSeriesUid = DicomUid.GenerateUid().UID;
                file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesUid);
                _seriesMap.Add(seriesUid, newSeriesUid);
            }
            else
            {
                string newSeriesUid = _seriesMap[seriesUid];
                file.DataSet[DicomTags.SeriesInstanceUid].SetStringValue(newSeriesUid);
            }
            DicomUid sopUid = DicomUid.GenerateUid();
            file.MediaStorageSopInstanceUid = sopUid.UID;
            file.DataSet[DicomTags.SopInstanceUid].SetStringValue(sopUid.UID);
            
            
            file.DataSet[DicomTags.PatientId].SetStringValue(PatientsId.Text);
            file.DataSet[DicomTags.IssuerOfPatientId].SetStringValue(IssuerOfPatientsId.Text);
            file.DataSet[DicomTags.PatientsName].SetStringValue(PatientsName.Text);
            file.DataSet[DicomTags.PatientsBirthDate].SetStringValue(PatientsBirthdate.Text);
            file.DataSet[DicomTags.PatientsSex].SetStringValue(PatientsSex.Text);
            file.DataSet[DicomTags.AccessionNumber].SetStringValue(AccessionNumber.Text);
            file.DataSet[DicomTags.StudyDate].SetStringValue(StudyDate.Text);
            file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(StudyInstanceUid.Text);

            file.DataSet[DicomTags.SeriesDescription].SetStringValue(seriesDescription);

        }

        public static DialogResult InputBox(string title, string promptText, ref string value)
        {
            Form form = new Form();
            Label label = new Label();
            TextBox textBox = new TextBox();
            Button buttonOk = new Button();
            Button buttonCancel = new Button();

            form.Text = title;
            label.Text = promptText;
            textBox.Text = value;

            buttonOk.Text = "OK";
            buttonCancel.Text = "Cancel";
            buttonOk.DialogResult = DialogResult.OK;
            buttonCancel.DialogResult = DialogResult.Cancel;

            label.SetBounds(9, 20, 372, 13);
            textBox.SetBounds(12, 36, 372, 20);
            buttonOk.SetBounds(228, 72, 75, 23);
            buttonCancel.SetBounds(309, 72, 75, 23);

            label.AutoSize = true;
            textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

            form.ClientSize = new Size(396, 107);
            form.Controls.AddRange(new Control[] { label, textBox, buttonOk, buttonCancel });
            form.ClientSize = new Size(Math.Max(300, label.Right + 10), form.ClientSize.Height);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.StartPosition = FormStartPosition.CenterScreen;
            form.MinimizeBox = false;
            form.MaximizeBox = false;
            form.AcceptButton = buttonOk;
            form.CancelButton = buttonCancel;

            DialogResult dialogResult = form.ShowDialog();
            value = textBox.Text;
            return dialogResult;
        }

        private void LoadSamples_Click(object sender, EventArgs e)
        {
            // Build the index
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                _seriesToFilesMap = new Dictionary<string, List<string>>();

                string folder = folderBrowserDialog1.SelectedPath;
                FileProcessor.Process(folder, "*.dcm",
                      delegate(string path)
                      {

                          DicomFile file = new DicomFile(path);
                          file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

                          string seriesUid =
                              file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                          if (!_seriesToFilesMap.ContainsKey(seriesUid))
                          {
                              _seriesToFilesMap.Add(seriesUid, new List<String>());
                          }

                          _seriesToFilesMap[seriesUid].Add(path);

                      },
                      true);

            }
        }

        private void RandomPatient_Click(object sender, EventArgs e)
        {
            InitNewPatient();
        }

        private void InitNewPatient()
        {
            Random ran = new Random();
            PatientsName.Text =
                String.Format("{0}^{1}", _lastNames[ran.Next(_lastNames.Count)],
                              _givenNames[ran.Next(_givenNames.Count)]);

            PatientsId.Text = String.Format("{0}{1}{2}-{3}", 
                    (char)((int)'A' +  ran.Next(26)),
                    (char)((int)'A' +  ran.Next(26)),
                    (char)((int)'A' +  ran.Next(26)),
                    ran.Next(1000, 9999));
            PatientsBirthdate.Text = ran.Next() % 10 == 0
                                         ? ""
                                         : DateParser.ToDicomString(new DateTime(1900, 1, 1).AddDays(ran.Next(0, 36000)));

        }

        private void InitNewStudy()
        {
            Random ran = new Random();
            AccessionNumber.Text = String.Format("{0}{1}",  (char)((int)'A' +  ran.Next(26)), ran.Next(10000, 99999));
            StudyInstanceUid.Text = DicomUid.GenerateUid().UID;
            StudyDate.Text = ran.Next() % 10 == 0
                               ? ""
                               : DateParser.ToDicomString(new DateTime(1990, 1, 1).AddDays(ran.Next(0, 5000)));

        }

        private void NewStudy_Click(object sender, EventArgs e)
        {
            InitNewStudy();
        }
    }

    
}