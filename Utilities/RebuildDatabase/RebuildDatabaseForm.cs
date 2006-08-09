using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace ClearCanvas.Utilities.RebuildDatabase
{
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using System.IO;

    public partial class RebuildDatabaseForm : Form
    {
        delegate void UpdateProgressBarDelegate(ImageInsertCompletingEventArgs args);
        delegate void EndRebuildDelegate(DatabaseRebuildCompletedEventArgs args);

        public RebuildDatabaseForm()
        {
            InitializeComponent();
            Binding bindingConnectionString = new Binding("Text", Properties.Settings.Default, "ConnectionString", false, DataSourceUpdateMode.OnPropertyChanged);
            Binding bindingImageStoragePath = new Binding("Text", Properties.Settings.Default, "ImageStoragePath", false, DataSourceUpdateMode.OnPropertyChanged);
            Binding bindingRecursiveSearch = new Binding("Checked", Properties.Settings.Default, "RecursiveSearch", false, DataSourceUpdateMode.OnPropertyChanged);
            _connectionStringText.DataBindings.Add(bindingConnectionString);
            _imageFolderText.DataBindings.Add(bindingImageStoragePath);
            _findFilesRecursivelyCheckbox.DataBindings.Add(bindingRecursiveSearch);
            this.CancelButton = _exitButton;
            _stopButton.Enabled = false;
        }

        private void Browse_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            folderDialog.Description = "Choose the folder where images are stored";
            folderDialog.RootFolder = Environment.SpecialFolder.MyComputer;
          
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                _imageFolderText.Text = folderDialog.SelectedPath;
            }
        }

        private void RebuildDatabase_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.Save();
        }

        private void _exitButton_Click(object sender, EventArgs e)
        {
            if (null != _rebuilder && _rebuilder.IsRebuilding)
                _rebuilder.StopRebuild();
            Close();
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            _rebuilder = new DatabaseRebuilder(_connectionStringText.Text, _imageFolderText.Text, _findFilesRecursivelyCheckbox.Checked);
            _rebuilder.ImageInsertCompletingEvent += ImageInsertCompletingEventHandler;
            _rebuilder.DatabaseRebuildCompletedEvent += DatabaseRebuildCompletedEventHandler;

            // set up progress bar
            _progressBar.Visible = true;
            _progressBar.Minimum = 1;
            _progressBar.Maximum = _rebuilder.NumberOfFiles;
            _progressBar.Value = 1;
            _progressBar.Step = 1;

            _stopButton.Enabled = true;
            _exitButton.Enabled = false;
            _startTime = DateTime.Now;
            _rebuilder.StartRebuild();
        }

        public void ImageInsertCompletingEventHandler(Object source, ImageInsertCompletingEventArgs args)
        {
            BeginInvoke(new UpdateProgressBarDelegate(UpdateProgressBar), new object[] { args });
        }

        public void DatabaseRebuildCompletedEventHandler(Object source, DatabaseRebuildCompletedEventArgs args)
        {
            BeginInvoke(new EndRebuildDelegate(EndRebuild), new object[] { args });
        }

        private void _stopButton_Click(object sender, EventArgs e)
        {
            _rebuilder.StopRebuild();
        }

        private void UpdateProgressBar(ImageInsertCompletingEventArgs args)
        {
            _progressBar.PerformStep();
            _completedRebuildFile.Text = args.FileName.Substring(args.FileName.LastIndexOf('\\') + 1);
        }

        private void EndRebuild(DatabaseRebuildCompletedEventArgs args)
        {
            if (!args.RebulidWasAborted)
            {
                _stopTime = DateTime.Now;
                TimeSpan duration = _stopTime - _startTime;
                _stopButton.Enabled = false;
                _exitButton.Enabled = true;
                _completedRebuildFile.Text = "Processed " + _rebuilder.NumberOfFiles.ToString() + " files in " + duration.ToString();
            }
            else
            {
                _stopButton.Enabled = false;
                _exitButton.Enabled = true;
                _completedRebuildFile.Text = "Rebuild was aborted";
            }
        }

        private DateTime _startTime;
        private DateTime _stopTime;
        private DatabaseRebuilder _rebuilder = null;

    }
}