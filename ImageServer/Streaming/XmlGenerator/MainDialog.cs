using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

using ClearCanvas.ImageServer;
using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Exceptions;
using ClearCanvas.ImageServer.Dicom.IO;
using ClearCanvas.ImageServer.Streaming;

namespace ClearCanvas.ImageServer.Streaming.XmlGenerator
{
    public partial class MainDialog : Form
    {
		StudyStream _theStream = new StudyStream();

        public MainDialog()
        {
            InitializeComponent();
        }


        private void ButtonLoadFile_Click(object sender, EventArgs e)
        {


            openFileDialog.DefaultExt = "dcm";
            openFileDialog.ShowDialog();

            DicomFile dicomFile = new DicomFile(openFileDialog.FileName);

            DicomReadOptions options = new DicomReadOptions();

            dicomFile.Load(options);
            


            _theStream.AddFile(dicomFile);

        }

		private void _buttonLoadDirectory_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.ShowDialog();

			String directory = folderBrowserDialog.SelectedPath;

			DirectoryInfo dir = new DirectoryInfo(directory);

			LoadFiles(dir);
			

		}


		private void LoadFiles(DirectoryInfo dir)
		{
         
			FileInfo[] files = dir.GetFiles();

			foreach (FileInfo file in files)
			{

				Dicom.DicomFile dicomFile = new Dicom.DicomFile(file.FullName);

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
			saveFileDialog.ShowDialog();

			String file = saveFileDialog.FileName;

			XmlDocument doc = _theStream.GetMomento();

			StreamWriter writer = new StreamWriter(file);

			XmlWriterSettings xmlSettings = new XmlWriterSettings();

			xmlSettings.Encoding = Encoding.UTF8;
			xmlSettings.ConformanceLevel = ConformanceLevel.Document;
			xmlSettings.Indent = false;
			xmlSettings.NewLineOnAttributes = false;
			xmlSettings.CheckCharacters = true;
			xmlSettings.IndentChars = "";

			XmlWriter tw = XmlWriter.Create(writer, xmlSettings);

			doc.WriteTo(tw);

			tw.Close();
			tw = null;
			writer.Close();

		}

        private void _buttonLoadXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "xml";
            openFileDialog.ShowDialog();

            XmlDocument theDoc = new XmlDocument();

            theDoc.Load(openFileDialog.FileName);

            _theStream = new StudyStream();

            _theStream.SetMemento(theDoc);


        }
    }
}