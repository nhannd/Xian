#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using ClearCanvas.Dicom.Utilities.Xml;

namespace ClearCanvas.Dicom.XmlGenerator
{
    public partial class MainDialog : Form
    {
        StudyXml _theStream = new StudyXml();

        public MainDialog()
        {
            InitializeComponent();
        }


        private void ButtonLoadFile_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "dcm";
            openFileDialog.ShowDialog();

            DicomFile dicomFile = new DicomFile(openFileDialog.FileName);

            dicomFile.Load(DicomReadOptions.Default);

            _theStream.AddFile(dicomFile);
        }

        private void _buttonLoadDirectory_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.ShowDialog();

            String directory = folderBrowserDialog.SelectedPath;
			if (directory.Equals(String.Empty))
				return;

            DirectoryInfo dir = new DirectoryInfo(directory);

            LoadFiles(dir);
			

        }


        private void LoadFiles(DirectoryInfo dir)
        {
         
            FileInfo[] files = dir.GetFiles();

            foreach (FileInfo file in files)
            {

                DicomFile dicomFile = new DicomFile(file.FullName);

                try
                {

                    DicomReadOptions options = new DicomReadOptions();


                    dicomFile.Load(options);
                    _theStream.AddFile(dicomFile);
                    /*
					if (true == dicomFile.Load())
					{
						_theStream.AddFile(dicomFile);
					}
                     * */
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

        private void _buttonGenerateXml_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "xml";
            saveFileDialog.ShowDialog();

			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
        	settings.IncludeSourceFileName = false;
            XmlDocument doc = _theStream.GetMemento(settings);

            Stream fileStream = saveFileDialog.OpenFile();

            StudyXmlIo.Write(doc, fileStream);

            fileStream.Close();
        }

        private void _buttonLoadXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "xml";
            openFileDialog.ShowDialog();

            Stream fileStream = openFileDialog.OpenFile();

            XmlDocument theDoc = new XmlDocument();

            StudyXmlIo.Read(theDoc, fileStream);

            fileStream.Close();

            _theStream = new StudyXml();

            _theStream.SetMemento(theDoc);
        }

        private void _buttonGenerateGzipXml_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "gzip";

            saveFileDialog.ShowDialog();

            XmlDocument doc = _theStream.GetMemento(StudyXmlOutputSettings.None);

            Stream fileStream = saveFileDialog.OpenFile();

            StudyXmlIo.WriteGzip(doc, fileStream);

            fileStream.Close();
        }

        private void _buttonLoadGzipXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "gzip";
            openFileDialog.ShowDialog();

            Stream fileStream = openFileDialog.OpenFile();

            XmlDocument theDoc = new XmlDocument();

            StudyXmlIo.ReadGzip(theDoc, fileStream);

            fileStream.Close();

            _theStream = new StudyXml();

            _theStream.SetMemento(theDoc);
        }
    }
}