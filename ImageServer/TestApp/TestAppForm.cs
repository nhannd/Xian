#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Common.Actions;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;
using Rule=ClearCanvas.ImageServer.Rules.Rule;

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

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();

            DicomFile dicomFile = new DicomFile(openFileDialog.FileName);

            dicomFile.Load();

            ServerRulesEngine engine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SopProcessed"));
            engine.Load();

            engine.Execute(dicomFile);
            
        }
    }
}