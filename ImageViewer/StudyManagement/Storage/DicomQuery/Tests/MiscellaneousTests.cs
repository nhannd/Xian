#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca

// For information about the licensing and copyright of this software please
// contact ClearCanvas, Inc. at info@clearcanvas.ca

#endregion

#if UNIT_TESTS

using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class MiscellaneousTests
    {
        public void TestDicomTagPathGetAttribute_NoCreate()
        {
            var collection = new DicomAttributeCollection();
            var path = new DicomTagPath(DicomTags.PatientId);

            Assert.IsNull(path.GetAttribute(collection));

            collection[DicomTags.PatientId].SetStringValue("PatientId");
            Assert.IsNotNull(path.GetAttribute(collection));

            collection[DicomTags.PatientId].SetStringValue("PatientId");
            collection[DicomTags.PatientsName].SetStringValue("PatientsName");
            Assert.IsNotNull(path.GetAttribute(collection));

            path = new DicomTagPath(DicomTags.PatientsName);
            Assert.IsNotNull(path.GetAttribute(collection));

            path = new DicomTagPath(DicomTags.ViewCodeSequence);
            var sequence1 = new DicomSequenceItem();
            collection[DicomTags.ViewCodeSequence].AddSequenceItem(sequence1);
            Assert.IsNotNull(path.GetAttribute(collection));

            path += new DicomTagPath(DicomTags.StudyInstanceUid);
            sequence1[DicomTags.StudyInstanceUid].SetNullValue();
            Assert.IsNotNull(path.GetAttribute(collection));

            path = new DicomTagPath(new[]{DicomTags.ViewCodeSequence, DicomTags.ConceptNameCodeSequence});
            var sequence2 = new DicomSequenceItem();
            sequence2[DicomTags.CodeValue].SetStringValue("Code");
            sequence1[DicomTags.ConceptNameCodeSequence].AddSequenceItem(sequence2);
            Assert.IsNotNull(path.GetAttribute(collection));

            path += DicomTags.CodeValue;
            Assert.IsNotNull(path.GetAttribute(collection));
        }
    }
}

#endif