using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.Dicom.Utilities.Anonymization;

namespace ClearCanvas.ImageServer.TestApp
{

   
    public partial class TestReconcileForm : Form
    {
        private List<string> _files = new List<string>();

        public TestReconcileForm()
        {
            InitializeComponent();
        }

        private void Images_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                List<string> seriesUidList = new List<string>();

                Send.Enabled = false;
                SeriesTreeView.Nodes.Clear();

                _files = new List<string>();
                string folder = folderBrowserDialog1.SelectedPath;
                FileProcessor.Process(folder, "*.dcm",
                                      delegate(string path)
                                      {
                                          _files.Add(path);

                                          DicomFile file = new DicomFile(path);
                                          file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

                                          string seriesUid =
                                              file.DataSet[DicomTags.SeriesInstanceUid].GetString(0, String.Empty);
                                          if (!seriesUidList.Contains(seriesUid))
                                          {
                                              SeriesTreeView.Nodes.Add(seriesUid);
                                              seriesUidList.Add(seriesUid);
                                          }
                                          
                                      },
                                      true);


                if (_files.Count > 0)
                {
                    Send.Enabled = true;
                    DicomFile file = new DicomFile(_files[0]);
                    file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet);

                    if (MessageBox.Show("Use demographics data in these images?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        PatientsId.Text = file.DataSet[DicomTags.PatientId].GetString(0, String.Empty);
                        IssuerOfPatientsId.Text = file.DataSet[DicomTags.IssuerOfPatientId].GetString(0, String.Empty);
                        PatientsName.Text = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
                        PatientsBirthdate.Text = file.DataSet[DicomTags.PatientsBirthDate].GetString(0, String.Empty);
                        PatientsSex.Text = file.DataSet[DicomTags.PatientsSex].GetString(0, String.Empty);

                    }
                    if (MessageBox.Show("Use study information in these images?", "", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        AccessionNumber.Text = file.DataSet[DicomTags.AccessionNumber].GetString(0, String.Empty);
                        StudyDate.Text = file.DataSet[DicomTags.StudyDate].GetString(0, String.Empty);
                        StudyInstanceUid.Text = file.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
                    }
                }

                SeriesTab.Text = String.Format("Series ({0})", seriesUidList.Count);
            }

            
        }

        private void UpdateFile(DicomFile file)
        {
            file.DataSet[DicomTags.PatientId].SetStringValue(PatientsId.Text);
            file.DataSet[DicomTags.IssuerOfPatientId].SetStringValue(IssuerOfPatientsId.Text);
            file.DataSet[DicomTags.PatientsName].SetStringValue(PatientsName.Text);
            file.DataSet[DicomTags.PatientsBirthDate].SetStringValue(PatientsBirthdate.Text);
            file.DataSet[DicomTags.PatientsSex].SetStringValue(PatientsSex.Text);
            file.DataSet[DicomTags.AccessionNumber].SetStringValue(AccessionNumber.Text);
            file.DataSet[DicomTags.StudyDate].SetStringValue(StudyDate.Text);
            file.DataSet[DicomTags.StudyInstanceUid].SetStringValue(StudyInstanceUid.Text);
        }

        private void Send_Click(object sender, EventArgs e)
        {
            StorageScu scu = new StorageScu(LocalAE.Text, ServerAE.Text, ServerHost.Text, int.Parse(ServerPort.Text));
            foreach(string path in _files)
            {
                DicomFile file = new DicomFile(path);
                file.Load();

                UpdateFile(file);

                scu.AddStorageInstance(new StorageInstance(file));
            }

            scu.Send();
            scu.Join();

            MessageBox.Show(String.Format("{0} images sent", _files.Count));
        }
    }
}