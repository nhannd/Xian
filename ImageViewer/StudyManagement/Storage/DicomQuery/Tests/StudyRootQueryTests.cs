#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using System.Linq;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;
using System.IO;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class StudyRootQueryTests : ClearCanvas.Dicom.Tests.AbstractTest
    {
        private const string _testDatabaseFilename = "test_dicom_store.sdf";
        
        private const string _utf8 = "ISO_IR 192";
        private int _testStudyCount = 3;

        [TestFixtureSetUp]
        public void Setup()
        {
            DatabaseHelper.CreateDatabase(_testDatabaseFilename, DatabaseHelper.GetDatabaseFilePath(_testDatabaseFilename));
        }

        private static DataAccessContext CreateContext()
        {
            return new DataAccessContext(_testDatabaseFilename);
        }

        [Test]
        public void SelectAllStudies()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier();

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(_testStudyCount, results.Count);
            }
        }

        [Test]
        public void SelectPatientIdEquals()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier
                                   {
                                       PatientId = "12345678"
                                   };
                
                var results = query.StudyQuery(criteria);
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].PatientId);
            }
        }

        [Test]
        public void SelectPatientIdEmpty_NoCriteria()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = ""
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(3, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].PatientId);
            }
        }

        [Test]
        public void SelectPatientIdWildcard()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "123*"
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(2, results.Count);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "12345678") != null);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "123a5698") != null);
            }
        }

        [Test]
        public void SelectPatientIdWildcardSingle()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyRootQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "123?56?8"
                };

                var results = query.StudyQuery(criteria);
                Assert.AreEqual(2, results.Count);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "12345678") != null);
                Assert.IsTrue(results.SingleOrDefault(study => study.PatientId == "123a5698") != null);
            }
        }
    }
}
#endif