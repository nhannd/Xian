#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace ClearCanvas.Dicom.DataStore.Tests
{
	[TestFixture]
	public class ValidationTests
	{
		private PersistentObjectValidator _validator;

		public ValidationTests()
		{
		}

		[TestFixtureSetUp]
		public void Initialize()
		{
			Configuration configuration = new Configuration();
			string assemblyName = MethodBase.GetCurrentMethod().DeclaringType.Assembly.GetName().Name;
			configuration.Configure(@"..\" + assemblyName + ".cfg.xml");
			configuration.AddAssembly(assemblyName);

			_validator = new PersistentObjectValidator(configuration);
		}

		private static Study NewStudy()
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
		[ExpectedException(typeof(DataValidationException))]
		public void TestNullStudyInstanceUid()
		{
			Study study = NewStudy();
			study.StudyInstanceUid = null;
			_validator.ValidatePersistentObject(study);
		}

		[Test]
		[ExpectedException(typeof(DataValidationException))]
		public void TestEmptyStudyInstanceUid()
		{
			Study study = NewStudy();
			study.StudyInstanceUid = "";
			_validator.ValidatePersistentObject(study);
		}

		[Test]
		[ExpectedException(typeof(DataValidationException))]
		public void TestStudyFieldTooLong()
		{
			Study study = NewStudy(); 
			study.StudyTimeRaw = "GreaterThanSixteenCharacters";
			_validator.ValidatePersistentObject(study);
		}

		[Test]
		[ExpectedException(typeof(DataValidationException))]
		public void TestComponentFieldTooLong()
		{
			Study study = NewStudy(); 
			study.PatientId = "A string that is more than 64 characters in length should throw an exception";
			_validator.ValidatePersistentObject(study);
		}

		#endregion
	}
}

#endif