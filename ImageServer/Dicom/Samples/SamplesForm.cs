using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.ImageServer.Dicom.Samples
{
    public partial class SamplesForm : Form
    {
        private StorageScu _storageScu = new StorageScu();
        //private StorageScp _storageScp = new StorageScp();

        public SamplesForm()
        {
            InitializeComponent();

            SampleUtilities.RegisterLogHandler(this.OutputTextBox);
        }

        private void buttonStorageScuSelectFiles_Click(object sender, EventArgs e)
        {            
            this.openFileDialogStorageScu.ShowDialog();

            foreach (String file in this.openFileDialogStorageScu.FileNames)
            {
                _storageScu.AddFileToSend(file);
            }
            
        }

        private void buttonStorageScuConnect_Click(object sender, EventArgs e)
        {
            int port;
            if (!int.TryParse(_textBoxStorageScuRemotePort.Text,out port))
            {
                DicomLogger.LogError("Unable to parse port number: {0}", _textBoxStorageScuRemotePort.Text);
                return;
            }


            _storageScu.Send(_textBoxStorageScuRemoteAe.Text, _textBoxStorageScuRemoteHost.Text, port);
        }

        private void buttonStorageScpStartStop_Click(object sender, EventArgs e)
        {
            if (StorageScp.Started)
            {
                _buttonStorageScpStartStop.Text = "Start";
                StorageScp.StopListening(int.Parse(_textBoxStorageScpPort.Text));
            }
            else
            {
                _buttonStorageScpStartStop.Text = "Stop";
                StorageScp.StorageLocation = _textBoxStorageScpStorageLocation.Text;
                StorageScp.StartListening(_textBoxStorageScpAeTitle.Text,
                    int.Parse(_textBoxStorageScpPort.Text));

            }

        }

        private void buttonStorageScuSelectDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialogStorageScu.ShowDialog();

            _storageScu.AddDirectoryToSend(folderBrowserDialogStorageScu.SelectedPath);
        }

        private void _buttonStorageScuSelectStorageLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialogStorageScp.ShowDialog();

            _textBoxStorageScpStorageLocation.Text = folderBrowserDialogStorageScp.SelectedPath;
        }
    }
}