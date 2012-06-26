#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.ServiceModel.Query;
using ClearCanvas.ImageViewer.Common.Configuration.Tests;
using ClearCanvas.ImageViewer.Common.DicomServer.Tests;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    //TODO (Marmot):Test with Deleted and Reindex column set.

    [TestFixture]
    public class StudyStoreQueryTests
    {
        private const string _testDatabaseFilename = "test_store.sdf";

        [TestFixtureSetUp]
        public void Initialize()
        {
            var extensionFactory = new UnitTestExtensionFactory
                                       {
                                            { typeof(ServiceProviderExtensionPoint), typeof(TestSystemConfigurationServiceProvider) },
                                            { typeof(ServiceProviderExtensionPoint), typeof(DicomServerTestServiceProvider) },
                                            { typeof(ServiceProviderExtensionPoint), typeof(StudyStoreQueryServiceProvider) },
                                            { typeof (ServiceProviderExtensionPoint), typeof (ServerDirectoryServiceProvider) }
                                       };

            Platform.SetExtensionFactory(extensionFactory);

            if (!File.Exists(SqlCeDatabaseHelper<DicomStoreDataContext>.GetDatabaseFilePath(_testDatabaseFilename)))
                SqlCeDatabaseHelper<DicomStoreDataContext>.CreateDatabase(_testDatabaseFilename);
        }

        private static DataAccessContext CreateContext()
        {
            return new DataAccessContext(null, _testDatabaseFilename);
        }

        [Test]
        public void TestGetStudyCount()
        {
            using (var context = CreateContext())
            {
                var count = context.GetStudyStoreQuery().GetStudyCount();
                var realCount = context.GetStudyBroker().GetStudyCount();
                Assert.AreEqual(realCount, count);
            }
        }

        [Test]
        public void TestGetStudyCount_WithCriteria()
        {
            using (var context = CreateContext())
            {
                var count = context.GetStudyStoreQuery().GetStudyCount(
                    new StudyEntry
                        {
                            Study = new StudyRootStudyIdentifier{PatientId = "SCS*"}
                        });
                Assert.AreEqual(3, count);
            }
        }

        [Test]
        public void SelectPatientIdEquals()
        {
            using (var context = CreateContext())
            {
                var query = context.GetStudyStoreQuery();
                var criteria = new StudyRootStudyIdentifier
                {
                    PatientId = "12345678"
                };

                var results = query.GetStudyEntries(new StudyEntry{Study = criteria});
                Assert.AreEqual(1, results.Count);
                Assert.AreEqual(criteria.PatientId, results[0].Study.PatientId);
            }
        }
    }
}

#endif