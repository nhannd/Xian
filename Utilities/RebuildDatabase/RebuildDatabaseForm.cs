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
            Close();
        }

        private void _startButton_Click(object sender, EventArgs e)
        {
            DatabaseRebuilder rebuilder = new DatabaseRebuilder(_connectionStringText.Text, _imageFolderText.Text, _findFilesRecursivelyCheckbox.Checked);
            rebuilder.ImageInsertCompletingEvent += ImageInsertCompletingEventHandler;
            _progressBar.Visible = true;
            _progressBar.Minimum = 1;
            _progressBar.Maximum = rebuilder.NumberOfFiles;
            _progressBar.Value = 1;
            _progressBar.Step = 1;

            _stopButton.Enabled = false;
            _exitButton.Enabled = false;
            DateTime start = DateTime.Now;
            rebuilder.StartRebuild();
            DateTime stop = DateTime.Now;
            TimeSpan duration = stop - start;
            _stopButton.Enabled = true;
            _exitButton.Enabled = true;
            _completedRebuildFile.Text = "Processed " + rebuilder.NumberOfFiles.ToString() + " files in " + duration.ToString();
            _completedRebuildFile.Invalidate();
            _completedRebuildFile.Update();
        }

        public void ImageInsertCompletingEventHandler(Object source, ImageInsertCompletingEventArgs args)
        {
            _progressBar.PerformStep();
            _completedRebuildFile.Text = args.FileName;
            _completedRebuildFile.Update();
        }
    }
}