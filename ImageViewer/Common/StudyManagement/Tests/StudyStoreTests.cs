#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    [TestFixture]
    public class StudyStoreTests
    {
        [TestFixtureSetUp]
        public void Initialize1()
        {
            Platform.SetExtensionFactory(new UnitTestExtensionFactory
                                             {
                                                 { typeof(ServiceProviderExtensionPoint), typeof(StudyManagement.Tests.StudyStoreTestServiceProvider) }
                                             });

            //Force IsSupported to be re-evaluated.
            StudyStore.InitializeIsSupported();
        }

        public void Initialize2()
        {
            Platform.SetExtensionFactory(new NullExtensionFactory());
            //Force IsSupported to be re-evaluated.
            StudyStore.InitializeIsSupported();
        }

        [Test]
        public void TestStudyEntry_ToDicomAttributeCollection()
        {
            var entry = new StudyEntry
                                   {
                                       Study = new StudyRootStudyIdentifier
                                                   {
                                                       PatientId = "123"
                                                   },
                                       Data = new StudyEntryData
                                                  {
                                                      SourceAETitlesInStudy = new string[]{"PACS1", "PACS2"},
                                                      StationNamesInStudy = new string[] { "STN1", "STN2" },
                                                      InstitutionNamesInStudy = new string[] { "INST1", "INST2" },
                                                  }
                                   };

            var collection = entry.ToDicomAttributeCollection();

            Assert.AreEqual("PACS1\\PACS2", collection[DicomTags.SourceApplicationEntityTitle].ToString());
            Assert.AreEqual("STN1\\STN2", collection[DicomTags.StationName].ToString());
            Assert.AreEqual("INST1\\INST2", collection[DicomTags.InstitutionName].ToString());
        }

        [Test]
        public void TestIsSupported()
        {
            Initialize1();
            Assert.IsTrue(StudyStore.IsSupported);

            Initialize2();
            Assert.IsFalse(StudyStore.IsSupported);
        }
    }
}


#endif