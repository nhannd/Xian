#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Xml.Tests
{
	[TestFixture]
	public class GeneralStreamingTest : AbstractTest
	{
		[Test]
		public void ConstructorTest()
		{
			StudyXml stream = new StudyXml();

			stream = new StudyXml("1.1.1");

		}

		private void WriteStudyStream(string streamFile, StudyXml theStream)
		{
			StudyXmlOutputSettings settings = new StudyXmlOutputSettings();
			settings.IncludeSourceFileName = false;

			XmlDocument doc = theStream.GetMemento(settings);

			if (File.Exists(streamFile))
				File.Delete(streamFile);

			using (Stream fileStream = new FileStream(streamFile, FileMode.CreateNew))
			{
				StudyXmlIo.Write(doc, fileStream);
				fileStream.Close();
			}

			return;
		}
		/// <summary>
		/// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
		/// </summary>
		/// <param name="location">The location a study is stored.</param>
		/// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
		private StudyXml LoadStudyStream(string location)
		{
			StudyXml theXml = new StudyXml();

			if (File.Exists(location))
			{
				using (Stream fileStream = new FileStream(location, FileMode.Open))
				{
					XmlDocument theDoc = new XmlDocument();

					StudyXmlIo.Read(theDoc, fileStream);

					theXml.SetMemento(theDoc);

					fileStream.Close();
				}
			}


			return theXml;
		}

		[Test]
		public void CreationTest()
		{
			IList<DicomAttributeCollection> instanceList;

			string studyInstanceUid = DicomUid.GenerateUid().UID;

			instanceList = SetupMRSeries(4, 10, studyInstanceUid);



			StudyXml studyXml = new StudyXml(studyInstanceUid);

			string studyXmlFilename = Path.GetTempFileName();

			foreach (DicomAttributeCollection instanceCollection in instanceList)
			{
				instanceCollection[DicomTags.PixelData] = null;


				DicomFile theFile = new DicomFile("test", new DicomAttributeCollection(), DicomAttributeCollection.ToProvider(instanceCollection));
				SetupMetaInfo(theFile);

				studyXml.AddFile(theFile);

				WriteStudyStream(studyXmlFilename, studyXml);
			}

			StudyXml newXml = LoadStudyStream(studyXmlFilename);

			if (!Compare(newXml, instanceList))
				Assert.Fail("Comparison of StudyXML failed against base loaded from disk");

			if (!Compare(studyXml, instanceList))
				Assert.Fail("Comparison of StudyXML failed against base in memory");

		}

		private bool Compare(StudyXml studyXml, IList<DicomAttributeCollection> instanceList)
		{
			foreach (SeriesXml seriesXml in studyXml)
			{
				foreach (InstanceXml instanceXml in seriesXml)
				{
					foreach (DicomAttributeCollection baseCollection in instanceList)
					{
						string sopInstanceUid = baseCollection[DicomTags.SopInstanceUid].ToString();
						if (sopInstanceUid.Equals(instanceXml.SopInstanceUid))
						{
							if (!baseCollection.Equals(instanceXml.Collection))
							{
								return false;
							}
						}
					}
				}
			}
			return true;
		}
	}
}

#endif