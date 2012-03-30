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
            string filePath = DataAccessContext.GetDatabaseFilePath(_testDatabaseFilename);
            
            var resourceResolver = new ResourceResolver(typeof(StudyRootQueryTests).Assembly);
            using (Stream sourceStream = resourceResolver.OpenResource(Path.GetFileName(filePath)))
            {
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    var buffer = new byte[1024];
                    while (true)
                    {
                        int read = sourceStream.Read(buffer, 0, buffer.Length);
                        if (read <= 0)
                            break;

                        fileStream.Write(buffer, 0, read);
                        if (read < buffer.Length)
                            break;
                    }

                    fileStream.Close();
                }
            }
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