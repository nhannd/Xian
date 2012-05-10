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

namespace ClearCanvas.ImageViewer.StudyManagement.Core.Storage.DicomQuery.Tests
{
    [TestFixture]
    public class MiscellaneousTests
    {
        [Test]
        public void TestDicomTagPathGetAttribute_NoCreate()
        {
            var collection = new DicomAttributeCollection();
            var path = new DicomTagPath(DicomTags.PatientId);

            //PatientID IS empty
            Assert.IsNull(path.GetAttribute(collection));

            collection[DicomTags.PatientId].SetNullValue();
            //PatientID NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            collection[DicomTags.PatientId].SetStringValue("PatientId");
            collection[DicomTags.PatientsName].SetStringValue("PatientsName");
            //PatientID NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));
            
            path = new DicomTagPath(DicomTags.PatientsName);
            //PatientsName NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            //ViewCodeSequence IS empty
            path = new DicomTagPath(DicomTags.ViewCodeSequence);
            Assert.IsNull(path.GetAttribute(collection));
            
            var sequence1 = new DicomSequenceItem();
            collection[DicomTags.ViewCodeSequence].AddSequenceItem(sequence1);
            //ViewCodeSequence NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            path += DicomTags.CodeMeaning;
            //ViewCodeSequence/CodeMeaning IS empty
            Assert.IsNull(path.GetAttribute(collection));
            sequence1[DicomTags.CodeMeaning].SetNullValue();
            //ViewCodeSequence/CodeMeaning NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            //ViewCodeSequence/ConceptNameCodeSequence IS empty
            path = new DicomTagPath(DicomTags.ViewCodeSequence, DicomTags.ConceptNameCodeSequence);
            Assert.IsNull(path.GetAttribute(collection));

            var sequence2 = new DicomSequenceItem();
            sequence1[DicomTags.ConceptNameCodeSequence].AddSequenceItem(sequence2);

            //ViewCodeSequence/ConceptNameCodeSequence NOT empty
            Assert.IsNotNull(path.GetAttribute(collection));

            path += DicomTags.CodeValue;
            //ViewCodeSequence/ConceptNameCodeSequence/CodeValue IS empty
            Assert.IsNull(path.GetAttribute(collection));
            
            sequence2[DicomTags.CodeValue].SetStringValue("Code");
            //ViewCodeSequence/ConceptNameCodeSequence/CodeValue IS empty
            Assert.IsNotNull(path.GetAttribute(collection));
        }

        [Test]
        public void TestDicomTagPathGetAttribute_Create()
        {
            var collection = new DicomAttributeCollection();

            var path = new DicomTagPath(DicomTags.ViewCodeSequence);
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));

            path += DicomTags.CodeMeaning;
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));
            Assert.IsNotNull(path.UpOne().GetAttribute(collection));

            path = path.UpOne() + DicomTags.ConceptNameCodeSequence;
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));
            Assert.IsNotNull(path.UpOne().GetAttribute(collection));

            path += DicomTags.CodeValue;
            Assert.IsNull(path.GetAttribute(collection));
            Assert.IsNotNull(path.GetAttribute(collection, true));
            Assert.IsNotNull(path.UpOne().GetAttribute(collection));
        }
    }
}

#endif