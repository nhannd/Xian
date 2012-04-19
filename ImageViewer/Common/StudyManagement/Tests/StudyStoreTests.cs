#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.ServiceModel.Query;
using NUnit.Framework;

#if UNIT_TESTS

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Tests
{
    [TestFixture]
    public class StudyStoreTests
    {
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

            string ts = string.Format("{0}\\{1}", TransferSyntax.ExplicitVrLittleEndian.UidString, TransferSyntax.ImplicitVrLittleEndian.UidString);
            Assert.AreEqual(ts, collection[DicomTags.TransferSyntaxUid].ToString());

            Assert.AreEqual("PACS1\\PACS2", collection[DicomTags.SourceApplicationEntityTitle].ToString());
            Assert.AreEqual("STN1\\STN2", collection[DicomTags.StationName].ToString());
            Assert.AreEqual("INST1\\INST2", collection[DicomTags.InstitutionName].ToString());

            Assert.AreEqual("MONOCHROME1\\MONOCHROME2", collection[DicomTags.PhotometricInterpretation].ToString());
            Assert.AreEqual("16\\8", collection[DicomTags.BitsAllocated].ToString());
            Assert.AreEqual("16\\15\\11", collection[DicomTags.BitsStored].ToString());
        }
    }
}


#endif