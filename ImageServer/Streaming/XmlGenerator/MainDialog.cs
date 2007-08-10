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
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.IO;
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
            saveFileDialog.DefaultExt = "xml";
			saveFileDialog.ShowDialog();

			String file = saveFileDialog.FileName;

			XmlDocument doc = _theStream.GetMomento();

            Stream fileStream = saveFileDialog.OpenFile();

            StreamingIo.Write(doc, fileStream);

            fileStream.Close();
		}

        private void _buttonLoadXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "xml";
            openFileDialog.ShowDialog();

            Stream fileStream = openFileDialog.OpenFile();

            XmlDocument theDoc = new XmlDocument();

            StreamingIo.Read(theDoc, fileStream);

            fileStream.Close();

            _theStream = new StudyStream();

            _theStream.SetMemento(theDoc);
        }

        private void _buttonGenerateGzipXml_Click(object sender, EventArgs e)
        {
            saveFileDialog.DefaultExt = "gzip";

            saveFileDialog.ShowDialog();

            String file = saveFileDialog.FileName;

            XmlDocument doc = _theStream.GetMomento();

            Stream fileStream = saveFileDialog.OpenFile();

            StreamingIo.WriteGzip(doc, fileStream);

            fileStream.Close();
        }

        private void _buttonLoadGzipXml_Click(object sender, EventArgs e)
        {
            openFileDialog.DefaultExt = "gzip";
            openFileDialog.ShowDialog();

            Stream fileStream = openFileDialog.OpenFile();

            XmlDocument theDoc = new XmlDocument();

            StreamingIo.ReadGzip(theDoc, fileStream);

            fileStream.Close();

            _theStream = new StudyStream();

            _theStream.SetMemento(theDoc);
        }
    }
}