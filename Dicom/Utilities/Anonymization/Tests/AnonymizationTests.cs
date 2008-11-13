#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

#if UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587, 649

using System;
using System.Diagnostics;
using ClearCanvas.Dicom.Tests;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Utilities.Anonymization.Tests
{
	internal class UidData
	{
		[DicomField(DicomTags.StudyInstanceUid)] public string StudyInstanceUid;
		[DicomField(DicomTags.SeriesInstanceUid)] public string SeriesInstanceUid;
		[DicomField(DicomTags.SopInstanceUid)] public string SopInstanceUid;
		
		[DicomField(DicomTags.ReferencedSopInstanceUid)] public string ReferencedSopInstanceUid;
		[DicomField(DicomTags.FrameOfReferenceUid)] public string FrameOfReferenceUid;
		[DicomField(DicomTags.SynchronizationFrameOfReferenceUid)] public string SynchronizationFrameOfReferenceUid;
		[DicomField(DicomTags.Uid)] public string Uid;
		[DicomField(DicomTags.ReferencedFrameOfReferenceUid)] public string ReferencedFrameOfReferenceUid;
		[DicomField(DicomTags.RelatedFrameOfReferenceUid)] public string RelatedFrameOfReferenceUid;
	}

	internal class DateData
	{
		[DicomField(DicomTags.InstanceCreationDate)] public string InstanceCreationDate;
		[DicomField(DicomTags.SeriesDate)] public string SeriesDate;
		[DicomField(DicomTags.ContentDate)] public string ContentDate;
		[DicomField(DicomTags.AcquisitionDatetime)] public string AcquisitionDatetime;
	}

	[TestFixture]
	public class AnonymizationTests : AbstractTest
	{
		private DicomFile _file;

		private UidData _originalUidData;
		private StudyData _originaStudyData;
		private SeriesData _originaSeriesData;
		private DateData _originalDateData;

		private UidData _anonymizedUidData;
		private StudyData _anonymizedStudyData;
		private SeriesData _anonymizedSeriesData;
		private DateData _anonymizedDateData;

		public AnonymizationTests()
		{
		}

		private DicomFile CreateTestFile()
		{
			DicomFile file = new DicomFile();

			SetupMultiframeXA(file.DataSet, 128, 128, 2);

			file.DataSet[DicomTags.PatientsName].SetStringValue("Doe^Jon^Mr");

			//NOTE: this is not intended to be a realistic dataset; we're just testing the anonymizer.
			
			//add a couple of the dates that get adjusted.
			file.DataSet[DicomTags.ContentDate].SetStringValue("20080219");
			file.DataSet[DicomTags.AcquisitionDatetime].SetStringValue("20080219100406");

			//add a couple of the Uids that get remapped (ones that aren't already in the data set).
			file.DataSet[DicomTags.ReferencedSopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			file.DataSet[DicomTags.Uid].SetStringValue(DicomUid.GenerateUid().UID);

			//add a couple of the tags that get removed.
			file.DataSet[DicomTags.InstanceCreatorUid].SetStringValue(DicomUid.GenerateUid().UID);
			file.DataSet[DicomTags.StorageMediaFileSetUid].SetStringValue(DicomUid.GenerateUid().UID);

			//add a couple of the tags that get nulled.
			file.DataSet[DicomTags.StationName].SetStringValue("STATION1");
			file.DataSet[DicomTags.PatientComments].SetStringValue("Claustrophobic");

			//sequence
			DicomSequenceItem item = new DicomSequenceItem();
			file.DataSet[DicomTags.ReferencedImageSequence].AddSequenceItem(item);
			item[DicomTags.ReferencedSopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			item[DicomTags.ReferencedFrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
			//will be removed
			item[DicomTags.InstanceCreatorUid].SetStringValue(DicomUid.GenerateUid().UID);

			//nested sequence; will be removed
			item[DicomTags.RequestAttributesSequence].AddSequenceItem(item = new DicomSequenceItem());
			item[DicomTags.RequestedProcedureId].SetStringValue("XA123");
			item[DicomTags.ScheduledProcedureStepId].SetStringValue("XA1234");
			return file;
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestStrict()
		{
			DicomFile file = CreateTestFile();
			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;

			try
			{
				//this ought to work.
				anonymizer.Anonymize(file);
			}
			catch(Exception)
			{
				Assert.Fail("Not strict - no exception expected.");
			}

			anonymizer = new DicomAnonymizer();
			Assert.IsTrue(anonymizer.ValidationOptions == ValidationOptions.Default); //strict by default

			//should throw.
			anonymizer.Anonymize(CreateTestFile());
		}

		[Test]
		public void TestSimple()
		{
			Initialize();

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.RelaxAllChecks;
			anonymizer.Anonymize(_file);

			AfterAnonymize(new StudyData(), new SeriesData());

			ValidateNullDates(_anonymizedDateData);
		}

		[Test]
		public void TestPrototypes()
		{
			Initialize();

			StudyData studyPrototype = new StudyData();
			studyPrototype.PatientId = "123";
			studyPrototype.PatientsBirthDateRaw = "19760810";
			studyPrototype.PatientsNameRaw = "Patient^Anonymous";
			studyPrototype.PatientsSex = "M";
			studyPrototype.StudyDateRaw = "20080220";
			studyPrototype.StudyDescription= "Test";
			studyPrototype.StudyId = "Test";

			SeriesData seriesPrototype = new SeriesData();
			seriesPrototype.SeriesDescription = "Series";
			seriesPrototype.ProtocolName = "Protocol";
			seriesPrototype.SeriesNumber = "1";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.SeriesDataPrototype = seriesPrototype;
			anonymizer.Anonymize(_file);

			AfterAnonymize(studyPrototype, seriesPrototype);

			//validate the adjusted dates.
			Assert.AreEqual(_anonymizedDateData.InstanceCreationDate, "20080220");
			Assert.AreEqual(_anonymizedDateData.SeriesDate, "20080220");

			Assert.AreEqual(_anonymizedDateData.ContentDate, "20080220");
			Assert.AreEqual(_anonymizedDateData.AcquisitionDatetime, "20080220100406");
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidatePatientIdNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientId = "123";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidatePatientIdNotEmpty() {
			Initialize();

			_file.DataSet[DicomTags.PatientId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientId = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void TestValidatePatientIdAllowEmpty() {
			Initialize();

			_file.DataSet[DicomTags.PatientId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientId = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.AllowEmptyPatientId;
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidatePatientsNameNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Anonymous^Mr");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsNameRaw = "PATIENT^ANONYMOUS";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidatePatientsNameNotEmpty() {
			Initialize();

			_file.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Anonymous^Mr");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsNameRaw = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void TestValidatePatientsNameAllowEmpty() {
			Initialize();

			_file.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Anonymous^Mr");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsNameRaw = "";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.AllowEmptyPatientName;
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidateAccessionNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.AccessionNumber].SetStringValue("1234");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.AccessionNumber = "1234";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidatePatientsBirthDateNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.PatientsBirthDate].SetStringValue("19760810");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsBirthDateRaw = "19760810";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		public void TestValidatePatientsBirthDateAllowEqual() {
			Initialize();

			_file.DataSet[DicomTags.PatientsBirthDate].SetStringValue("19760810");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.PatientsBirthDateRaw = "19760810";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.ValidationOptions = ValidationOptions.AllowEqualBirthDate;
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		[Test]
		[ExpectedException(typeof(DicomAnonymizerValidationException))]
		public void TestValidateStudyIdNotEqual()
		{
			Initialize();

			_file.DataSet[DicomTags.StudyId].SetStringValue("123");
			StudyData studyPrototype = CreateStudyPrototype();
			studyPrototype.StudyId = "123";

			DicomAnonymizer anonymizer = new DicomAnonymizer();
			anonymizer.StudyDataPrototype = studyPrototype;
			anonymizer.Anonymize(_file);
		}

		private void Initialize()
		{
			_file = CreateTestFile();
			
			_originalUidData = new UidData();
			_originaStudyData = new StudyData();
			_originaSeriesData = new SeriesData();
			_originalDateData = new DateData();

			_file.DataSet.LoadDicomFields(_originalUidData);
			_file.DataSet.LoadDicomFields(_originaStudyData);
			_file.DataSet.LoadDicomFields(_originaSeriesData);
			_file.DataSet.LoadDicomFields(_originalDateData);
		}

		private void AfterAnonymize(StudyData studyPrototype, SeriesData seriesPrototype)
		{
			_anonymizedUidData = new UidData();
			_anonymizedStudyData = new StudyData();
			_anonymizedSeriesData = new SeriesData();
			_anonymizedDateData = new DateData();

			_file.DataSet.LoadDicomFields(_anonymizedUidData);
			_file.DataSet.LoadDicomFields(_anonymizedStudyData);
			_file.DataSet.LoadDicomFields(_anonymizedSeriesData);
			_file.DataSet.LoadDicomFields(_anonymizedDateData);

			ValidateRemovedTags(_file.DataSet);
			ValidateNulledAttributes(_file.DataSet);
			ValidateRemappedUids(_originalUidData, _anonymizedUidData);

			Assert.AreEqual(_anonymizedStudyData.PatientId, studyPrototype.PatientId);
			Assert.AreEqual(_anonymizedStudyData.PatientsNameRaw, studyPrototype.PatientsNameRaw);
			Assert.AreEqual(_anonymizedStudyData.PatientsBirthDateRaw, studyPrototype.PatientsBirthDateRaw);
			Assert.AreEqual(_anonymizedStudyData.PatientsSex, studyPrototype.PatientsSex);
			Assert.AreEqual(_anonymizedStudyData.AccessionNumber, studyPrototype.AccessionNumber);
			Assert.AreEqual(_anonymizedStudyData.StudyDateRaw, studyPrototype.StudyDateRaw);
			Assert.AreEqual(_anonymizedStudyData.StudyDescription, studyPrototype.StudyDescription);
			Assert.AreEqual(_anonymizedStudyData.StudyId, studyPrototype.StudyId);

			Assert.AreEqual(_anonymizedSeriesData.SeriesDescription, seriesPrototype.SeriesDescription);
			Assert.AreEqual(_anonymizedSeriesData.ProtocolName, seriesPrototype.ProtocolName);
			Assert.AreEqual(_anonymizedSeriesData.SeriesNumber, seriesPrototype.SeriesNumber);
		}

		private static StudyData CreateStudyPrototype()
		{
			StudyData studyPrototype = new StudyData();
			studyPrototype.AccessionNumber = "0x0A11BA5E";
			studyPrototype.PatientId = "216CA75";
			studyPrototype.PatientsBirthDate = DateTime.Now;
			studyPrototype.PatientsNameRaw = "PICARD^JEAN-LUC^^CPT.";
			studyPrototype.PatientsSex = "M";
			studyPrototype.StudyDate = DateTime.Now;
			studyPrototype.StudyDescription = "Description of a study prototype, anonymized";
			studyPrototype.StudyId = "STUDY158739";
			return studyPrototype;
		}

		private static void ValidateNullDates(DateData anonymizedDateData)
		{
			Assert.AreEqual(anonymizedDateData.InstanceCreationDate, "");
			Assert.AreEqual(anonymizedDateData.AcquisitionDatetime, "");
			Assert.AreEqual(anonymizedDateData.ContentDate, "");
			Assert.AreEqual(anonymizedDateData.SeriesDate, "");

			Trace.WriteLine("Validated Nulled Dates.");
		}

		private static void ValidateNulledAttributes(DicomAttributeCollection dataset)
		{
			//just test a couple
			Assert.AreEqual(dataset[DicomTags.StationName].ToString(), "");
			Assert.AreEqual(dataset[DicomTags.PatientComments].ToString(), "");

			Trace.WriteLine("Validated Nulled Attributes.");
		}

		private static void ValidateRemovedTags(DicomAttributeCollection dataset)
		{
			if (dataset.Contains(DicomTags.InstanceCreatorUid))
				throw new Exception("InstanceCreatorUid");
			if (dataset.Contains(DicomTags.StorageMediaFileSetUid))
				throw new Exception("StorageMediaFileSetUid");
			if (dataset.Contains(DicomTags.RequestAttributesSequence))
				throw new Exception("RequestAttributesSequence");

			DicomSequenceItem item = ((DicomSequenceItem[])dataset[DicomTags.ReferencedImageSequence].Values)[0];
			if (item.Contains(DicomTags.InstanceCreatorUid))
				throw new Exception("InstanceCreatorUid");

			if (item.Contains(DicomTags.RequestAttributesSequence))
				throw new Exception("RequestAttributesSequence");
		}

		private static void ValidateRemappedUids(UidData originalData, UidData anonymizedData)
		{
			if (originalData.StudyInstanceUid == anonymizedData.StudyInstanceUid)
				throw new Exception("StudyInstanceUid");

			if (originalData.SeriesInstanceUid == anonymizedData.SeriesInstanceUid)
				throw new Exception("SeriesInstanceUid");
			
			if (originalData.SopInstanceUid == anonymizedData.SopInstanceUid)
				throw new Exception("SopInstanceUid");

			if (!String.IsNullOrEmpty(originalData.ReferencedSopInstanceUid)
			    && originalData.ReferencedSopInstanceUid == anonymizedData.ReferencedSopInstanceUid)
				throw new Exception("ReferencedSopInstanceUid");

			if (!String.IsNullOrEmpty(originalData.FrameOfReferenceUid)
			    && originalData.FrameOfReferenceUid == anonymizedData.FrameOfReferenceUid)
				throw new Exception("FrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.SynchronizationFrameOfReferenceUid)
			    && originalData.SynchronizationFrameOfReferenceUid == anonymizedData.SynchronizationFrameOfReferenceUid)
				throw new Exception("SynchronizationFrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.Uid) && originalData.Uid == anonymizedData.Uid)
				throw new Exception("Uid");

			if (!String.IsNullOrEmpty(originalData.ReferencedFrameOfReferenceUid)
			    && originalData.ReferencedFrameOfReferenceUid == anonymizedData.ReferencedFrameOfReferenceUid)
				throw new Exception("ReferencedFrameOfReferenceUid");

			if (!String.IsNullOrEmpty(originalData.RelatedFrameOfReferenceUid)
			    && originalData.RelatedFrameOfReferenceUid == anonymizedData.RelatedFrameOfReferenceUid)
				throw new Exception("RelatedFrameOfReferenceUid");

			Trace.WriteLine("Validated Remapped Uids.");
		}
	}
}

#endif