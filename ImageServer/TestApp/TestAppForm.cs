using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.TestApp
{
    public partial class TestAppForm : Form
    {
        public TestAppForm()
        {
            
            InitializeComponent();
        }

        private void checkBoxLoadTest_CheckedChanged(object sender, EventArgs e)
        {
            try
            {

                FilesystemMonitor monitor = new FilesystemMonitor();

                monitor.Load();


                TypeEnum t = new TypeEnum();
                t.SetEnum(200);

                IReadContext read = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();

                IInsertStudyStorage insert = read.GetBroker<IInsertStudyStorage>();

                StudyStorageInsertParameters criteria = new StudyStorageInsertParameters();

                criteria.StudyInstanceUid = "1.2.3.4";
                criteria.FilesystemKey = monitor.Filesystems[0].Filesystem.GetKey();
                criteria.Folder = "20070101";
                criteria.ServerPartitionKey = monitor.Partitions[0].GetKey();

                IList<StudyStorageLocation> storage = insert.Execute(criteria);

                StudyStorageLocation storageEntry = storage[0];
            }
            catch (Exception x)
            {
                Platform.Log(LogLevel.Error, x);
            }
        }

        private void buttonSelectDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();

            folderBrowserDialog.ShowDialog();

            String directory = folderBrowserDialog.SelectedPath;

            DirectoryInfo dir = new DirectoryInfo(directory);

            LoadFiles(dir);
        }

        private void SearchAttributeSet(DicomAttributeCollection set, string filename)
        {
            foreach (DicomAttribute attrib in set)
            {
                if (attrib.Tag.IsPrivate && attrib.Tag.VR.Equals(DicomVr.SQvr))
                {
                    DicomLogger.LogInfo("Found file with private SQ: {0}", filename);
                    return;
                }
                else if (attrib.Tag.VR.Equals(DicomVr.SQvr) && !attrib.IsNull)
                {
                    // Recursive search
                    foreach (DicomSequenceItem item in (DicomSequenceItem[])attrib.Values)
                    {
                        SearchAttributeSet(item,filename);
                    }
                }
            }
        }

        private void LoadFiles(DirectoryInfo dir)
        {

            FileInfo[] files = dir.GetFiles();

            DicomLogger.LogInfo("Scanning directory: {0}", dir.FullName);

            foreach (FileInfo file in files)
            {

                DicomFile dicomFile = new DicomFile(file.FullName);

                try
                {
                    DicomReadOptions options = new DicomReadOptions();

                    dicomFile.Load(options);

                    SearchAttributeSet(dicomFile.DataSet,dicomFile.Filename);
                }
                catch (DicomException)
                {
                    // TODO:  Add some logging for failed files
                }

            }

            String[] subdirectories = Directory.GetDirectories(dir.FullName);
            foreach (String subPath in subdirectories)
            {
                DirectoryInfo subDir = new DirectoryInfo(subPath);
                LoadFiles(subDir);
                continue;
            }

        }
    }
}