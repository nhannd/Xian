using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ClearCanvas.Dicom.Samples
{
    public partial class SamplesForm : Form
    {
        #region Private Constants
        private const string STR_Cancel = "Cancel";
        private const string STR_Verify = "Verify";
        #endregion

        private StorageScu _storageScu = new StorageScu();
        private VerificationScu _verificationScu = new VerificationScu();

        public SamplesForm()
        {
            InitializeComponent();
            _buttonStorageScuVerify.Text = STR_Verify;

            SampleUtilities.RegisterLogHandler(this.OutputTextBox);

            if (String.IsNullOrEmpty(Properties.Settings.Default.ScpStorageFolder))
            {
                Properties.Settings.Default.ScpStorageFolder = Path.Combine(Path.GetTempPath(), "DicomImages");
            }
        }

        #region Button Click Handlers
        private void buttonStorageScuSelectFiles_Click(object sender, EventArgs e)
        {            
            this.openFileDialogStorageScu.ShowDialog();

            foreach (String file in this.openFileDialogStorageScu.FileNames)
            {
                if (file != null)
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


            _storageScu.Send(_textBoxStorageScuLocalAe.Text, _textBoxStorageScuRemoteAe.Text, _textBoxStorageScuRemoteHost.Text, port);
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
            if (folderBrowserDialogStorageScu.SelectedPath == null)
                return;

            _storageScu.AddDirectoryToSend(folderBrowserDialogStorageScu.SelectedPath);
        }

        private void _buttonStorageScuSelectStorageLocation_Click(object sender, EventArgs e)
        {
            folderBrowserDialogStorageScp.ShowDialog();

            _textBoxStorageScpStorageLocation.Text = folderBrowserDialogStorageScp.SelectedPath;
        }

        private void _buttonStorageScuVerify_Click(object sender, EventArgs e)
        {
            if (_buttonStorageScuVerify.Text == STR_Verify)
                StartVerify();
            else
                CancelVerify();
        }

        private void _buttonOutputClearLog_Click(object sender, EventArgs e)
        {
            OutputTextBox.Text = "";
        }

        private void _buttonStorageScuClearFiles_Click(object sender, EventArgs e)
        {
            _storageScu.ClearFiles();
        }
        #endregion

        private void StartVerify()
        {
            int port;
            if (!int.TryParse(_textBoxStorageScuRemotePort.Text, out port))
            {
                DicomLogger.LogError("Unable to parse port number: {0}", _textBoxStorageScuRemotePort.Text);
                return;
            }
            IAsyncResult o_AsyncResult = _verificationScu.BeginVerify(_textBoxStorageScuLocalAe.Text, _textBoxStorageScuRemoteAe.Text, _textBoxStorageScuRemoteHost.Text, port, new AsyncCallback(VerifyComplete), null);
            SetVerifyButton(true);
        }

        private void VerifyComplete(IAsyncResult ar)
        {
            VerificationResult verificationResult = _verificationScu.EndVerify(ar);
            DicomLogger.LogInfo("Verify result: " + verificationResult.ToString());
            SetVerifyButton(false);
        }


        private void SetVerifyButton(bool running)
        {
            if (!this.InvokeRequired)
            {
                if (running)
                    _buttonStorageScuVerify.Text = STR_Cancel;
                else
                    _buttonStorageScuVerify.Text = STR_Verify;
            }
            else
            {
                this.BeginInvoke(new Action<bool>(SetVerifyButton), new object[] { running });
            }
        }

        private void CancelVerify()
        {
            _verificationScu.Cancel();
        }
    }
}