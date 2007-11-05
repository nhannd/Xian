#if UNIT_TESTS

using NUnit.Framework;

namespace ClearCanvas.Dicom.DataStore.Tests
{
	[TestFixture]
	public class ValidationTests
	{
		private PersistenObjectValidator _validator;

		public ValidationTests()
		{
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			_validator = new PersistenObjectValidator(DataAccessLayer.HibernateConfiguration);
		}

		private Study NewStudy()
		{
			Study study = new Study();
			study.StudyInstanceUid = "123";
			study.AccessionNumber = "abc";
			return study;
		}

		#region Study Tests

		[Test]
		public void TestValidStudy()
		{
			_validator.ValidatePersistentObject(NewStudy());
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestNullStudyInstanceUid()
		{
			Study study = NewStudy();
			study.StudyInstanceUid = null;
			_validator.ValidatePersistentObject(study);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestEmptyStudyInstanceUid()
		{
			Study study = NewStudy();
			study.StudyInstanceUid = "";
			_validator.ValidatePersistentObject(study);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestStudyFieldTooLong()
		{
			Study study = NewStudy(); 
			study.StudyTimeRaw = "GreaterThanSixteenCharacters";
			_validator.ValidatePersistentObject(study);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestComponentFieldTooLong()
		{
			Study study = NewStudy(); 
			study.PatientId = new PatientId("A string that is more than 64 characters in length should throw an exception");
			_validator.ValidatePersistentObject(study);
		}

		#endregion

		#region Series Tests

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestNullSeriesInstanceUid()
		{
			Series series = new Series();
			series.SeriesInstanceUid = null;
			_validator.ValidatePersistentObject(series);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestEmptySeriesInstanceUid()
		{
			Series series = new Series();
			series.SeriesInstanceUid = "";
			_validator.ValidatePersistentObject(series);
		}

		#endregion

		#region Image Tests

		[Test]
		public void TestValidSopInstance()
		{
			ImageSopInstance sop = new ImageSopInstance();
			sop.SopInstanceUid = "123";
			sop.TransferSyntaxUid = "1.2.3.4"; //obviously not real
			sop.SopClassUid = "1.2.3";
			sop.InstanceNumber = 1;
			sop.SpecificCharacterSet = "";
			sop.LocationUri = new DicomUri("c:\\somewhere");

			sop.BitsAllocated = 16;
			sop.BitsStored = 16;
			sop.HighBit = 15;
			sop.PixelRepresentation = 1;
			sop.SamplesPerPixel = 1;
			sop.PlanarConfiguration = 0;
			sop.PhotometricInterpretation = PhotometricInterpretation.Monochrome2;
			sop.Rows = 256;
			sop.Columns = 256;
			sop.PixelSpacing = new PixelSpacing(1, 1);
			sop.PixelAspectRatio = new PixelAspectRatio(64, 64);
			sop.RescaleIntercept = 0;
			sop.RescaleSlope = 1;

			_validator.ValidatePersistentObject(sop);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestNullSopInstanceUid()
		{
			ImageSopInstance sop = new ImageSopInstance();
			sop.SopInstanceUid = null;
			_validator.ValidatePersistentObject(sop);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestEmptySopInstanceUid()
		{
			ImageSopInstance sop = new ImageSopInstance();
			sop.SopInstanceUid = "";
			_validator.ValidatePersistentObject(sop);
		}

		[Test]
		[ExpectedException(typeof(DataStoreException))]
		public void TestSopFieldTooLong()
		{
			ImageSopInstance sop = new ImageSopInstance();
			sop.SopInstanceUid = "123";
			sop.TransferSyntaxUid = "A string that is more than 64 characters in length should throw an exception";
			_validator.ValidatePersistentObject(sop);
		}

		#endregion
	}
}

#endif