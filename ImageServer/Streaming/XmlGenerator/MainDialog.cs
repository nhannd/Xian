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

        private void CreateFileTest()
        {
            DicomFile theFile = new DicomFile("CreateFileTest.dcm");

            Dicom.AttributeCollection dataSet = theFile.DataSet;

            Dicom.AttributeCollection metaInfo = theFile.DataSet;


            SetupMR(dataSet);

            SetupMetaInfo(theFile);

            bool result = theFile.Write();

            DicomFile newFile = new DicomFile("CreateFileTest.dcm");

            newFile.Load();

            newFile.DataSet.GetEnumerator();
            newFile.MetaInfo.GetEnumerator();

            bool test = dataSet.Equals(newFile.DataSet);
        }

        private void ButtonLoadFile_Click(object sender, EventArgs e)
        {
            CreateFileTest();


            openFileDialog.DefaultExt = "dcm";
            openFileDialog.ShowDialog();

            Dicom.DicomFile dicomFile = new Dicom.DicomFile(openFileDialog.FileName);

            if (false == dicomFile.Load())
            {
            }

            Dicom.AbstractAttribute patientsName = dicomFile.DataSet[DicomTags.PatientsName];

            Dicom.AbstractAttribute sopInstanceUID = dicomFile.DataSet[DicomTags.SOPInstanceUID];

            SopClass sop = dicomFile.SopClass;
            
            foreach (Dicom.AbstractAttribute attrib in dicomFile.DataSet)
            {
                if (attrib is AttributeSQ)
                {
                    SequenceItem[] items = (SequenceItem[])((AttributeSQ)attrib).Values;
                    foreach (SequenceItem theItem in items)
                    {
                        IEnumerator<Dicom.AbstractAttribute> enumerator = theItem.GetEnumerator();

                        Dicom.AbstractAttribute firstAttrib = enumerator.Current;
                    }
                }
            }

            // Forces a load/marshalling of the attributes
            dicomFile.MetaInfo.GetEnumerator();

            Dicom.AttributeCollection copy = dicomFile.DataSet.Copy(false);

            _theStream.AddFile(dicomFile);

        }

		private void _buttonLoadDirectory_Click(object sender, EventArgs e)
		{
			folderBrowserDialog.ShowDialog();

			String directory = folderBrowserDialog.SelectedPath;

			DirectoryInfo dir = new DirectoryInfo(directory);

			LoadFiles(dir);
			

		}

        private void SetupMR(Dicom.AttributeCollection theSet)
        {
            theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
            theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
            theSet[DicomTags.InstanceCreationDate].SetStringValue("20070618");
            theSet[DicomTags.InstanceCreationTime].SetStringValue("133600");
            theSet[DicomTags.SOPClassUID].SetStringValue("1.2.840.10008.5.1.3.1.1.4");
            theSet[DicomTags.SOPInstanceUID].SetStringValue("1.1.1.1.1");
            theSet[DicomTags.StudyDate].SetStringValue("20070618");
            theSet[DicomTags.StudyTime].SetStringValue("1336000");
            theSet[DicomTags.SeriesDate].SetStringValue("20070618");
            theSet[DicomTags.SeriesTime].SetStringValue("133700");
            theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
            theSet[DicomTags.Modality].SetStringValue("MR");
            theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
            theSet[DicomTags.InstitutionName].SetStringValue("Mount Sinai Hospital");
            theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
            theSet[DicomTags.StudyDescription].SetStringValue("HEART");
            theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
            theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
            theSet[DicomTags.PatientID].SetStringValue("ID123-45-9999");
            theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
            theSet[DicomTags.PatientsSex].SetStringValue("M");
            theSet[DicomTags.PatientsWeight].SetStringValue("70");
            theSet[DicomTags.SequenceVariant].SetStringValue("OTHER");
            theSet[DicomTags.ScanOptions].SetStringValue("CG");
            theSet[DicomTags.MRAcquisitionType].SetStringValue("2D");
            theSet[DicomTags.SliceThickness].SetStringValue("10.000000");
            theSet[DicomTags.RepetitionTime].SetStringValue("857.142883");
            theSet[DicomTags.EchoTime].SetStringValue("8.712100");
            theSet[DicomTags.NumberofAverages].SetStringValue("1");
            theSet[DicomTags.ImagingFrequency].SetStringValue("63.901150");
            theSet[DicomTags.ImagedNucleus].SetStringValue("1H");
            theSet[DicomTags.EchoNumbers].SetStringValue("1");
            theSet[DicomTags.MagneticFieldStrength].SetStringValue("1.500000");
            theSet[DicomTags.SpacingBetweenSlices].SetStringValue("10.00000");
            theSet[DicomTags.NumberofPhaseEncodingSteps].SetStringValue("81");
            theSet[DicomTags.EchoTrainLength].SetStringValue("0");
            theSet[DicomTags.PercentSampling].SetStringValue("63.281250");
            theSet[DicomTags.PercentPhaseFieldofView].SetStringValue("68.75000");
            theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
            theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
            theSet[DicomTags.ProtocolName].SetStringValue("2D EPI BH");
            theSet[DicomTags.TriggerTime].SetStringValue("14.000000");
            theSet[DicomTags.LowRRValue].SetStringValue("948");
            theSet[DicomTags.HighRRValue].SetStringValue("1178");
            theSet[DicomTags.IntervalsAcquired].SetStringValue("102");
            theSet[DicomTags.IntervalsRejected].SetStringValue("0");
            theSet[DicomTags.HeartRate].SetStringValue("56");
            theSet[DicomTags.ReceiveCoilName].SetStringValue("B");
            theSet[DicomTags.TransmitCoilName].SetStringValue("B");
            theSet[DicomTags.InplanePhaseEncodingDirection].SetStringValue("COL");
            theSet[DicomTags.FlipAngle].SetStringValue("50.000000");
            theSet[DicomTags.PatientPosition].SetStringValue("HFS");
            theSet[DicomTags.StudyInstanceUID].SetStringValue("1.1.1.1.1.2");
            theSet[DicomTags.SeriesInstanceUID].SetStringValue("1.1.1.1.1.3");
            theSet[DicomTags.StudyID].SetStringValue("1933");
            theSet[DicomTags.SeriesNumber].SetStringValue("1");
            theSet[DicomTags.AcquisitionNumber].SetStringValue("7");
            theSet[DicomTags.InstanceNumber].SetStringValue("1");
            theSet[DicomTags.ImagePositionPatient].SetStringValue("-61.7564\\-212.04848\\-99.6208");
            theSet[DicomTags.ImageOrientationPatient].SetStringValue("0.861\\0.492\\0.126\\-0.2965");
            theSet[DicomTags.FrameofReferenceUID].SetStringValue("1.1.1.1.1.4");
            theSet[DicomTags.PositionReferenceIndicator].SetStringValue(null);
            theSet[DicomTags.ImageComments].SetStringValue("Test MR Image");
            theSet[DicomTags.SamplesperPixel].SetStringValue("1");
            theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
            theSet[DicomTags.Rows].SetStringValue("256");
            theSet[DicomTags.Columns].SetStringValue("256");
            theSet[DicomTags.PixelSpacing].SetStringValue("1.367188");
            theSet[DicomTags.BitsAllocated].SetStringValue("16");
            theSet[DicomTags.BitsStored].SetStringValue("12");
            theSet[DicomTags.HighBit].SetStringValue("11");
            theSet[DicomTags.PixelRepresentation].SetStringValue("0");
            theSet[DicomTags.WindowCenter].SetStringValue("238");
            theSet[DicomTags.WindowWidth].SetStringValue("471");

            uint length = 256 * 256 * 2;

            AttributeOW pixels = new AttributeOW(DicomTags.PixelData); ;

            byte[] pixelArray = new byte[length];

            for (uint i = 0; i < length; i += 2)
                pixelArray[i] = (byte)(i % 255);

            pixels.Values = pixelArray;

            theSet[DicomTags.PixelData] = pixels;

        }
        private void SetupMetaInfo(DicomFile theFile)
        {

            Dicom.AttributeCollection theSet = theFile.MetaInfo;

            theSet[DicomTags.MediaStorageSOPClassUID] = theFile.DataSet[DicomTags.SOPClassUID].Copy();
            theSet[DicomTags.MediaStorageSOPInstanceUID] = theFile.DataSet[DicomTags.SOPInstanceUID].Copy();
            theFile.TransferSyntax = TransferSyntax.GetTransferSyntax(TransferSyntax.ExplicitVRLittleEndian); ;

            theSet[DicomTags.ImplementationClassUID].SetStringValue("1.1.1.1.1.11.1");
            theSet[DicomTags.ImplementationVersionName].SetStringValue("CC Pacs 1.0");
        }

		private void LoadFiles(DirectoryInfo dir)
		{
         
			FileInfo[] files = dir.GetFiles();

			foreach (FileInfo file in files)
			{

				Dicom.DicomFile dicomFile = new Dicom.DicomFile(file.FullName);

				try
				{
					if (true == dicomFile.Load())
					{
						_theStream.AddFile(dicomFile);
					}
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