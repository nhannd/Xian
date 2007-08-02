#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom.Tests
{
    [TestFixture]
    public class AttributeTests : AbstractTest
    {
        #region DicomAttributeAE Test
        [Test]
        public void AttributeAETest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeAE attrib = new DicomAttributeAE(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeAE attrib = new DicomAttributeAE(DicomTagDictionary.GetDicomTag(DicomTags.RetrieveAETitle));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);

        }
        #endregion

        #region DicomAttributeAS Test
        [Test]
        public void AttributeASTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeAS attrib = new DicomAttributeAS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeAS attrib = new DicomAttributeAS(DicomTagDictionary.GetDicomTag(DicomTags.PatientsAge));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion


        [Test]
        public void AttributeATTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeAT attrib = new DicomAttributeAT(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeAT attrib = new DicomAttributeAT(DicomTagDictionary.GetDicomTag(DicomTags.FrameIncrementPointer));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }

        #region DicomAttributeCS Test
        [Test]
        public void AttributeCSTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeCS attrib = new DicomAttributeCS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeCS attrib = new DicomAttributeCS(DicomTagDictionary.GetDicomTag(DicomTags.ImageType));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region DicomAttributeDA Test
        [Test]
        public void AttributeDATest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeDA attrib = new DicomAttributeDA(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeDA attrib = new DicomAttributeDA(DicomTagDictionary.GetDicomTag(DicomTags.StudyDate));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region DicomAttributeDS Test
        [Test]
        public void AttributeDSTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.WindowCenter));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region SpecificCharacterSet Test
        [Test]
        public void SpecificCharacterSetTest()
        {
            bool testResult = false;
            try
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.AccessionNumber));
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                DicomAttributeDS attrib = new DicomAttributeDS(DicomTagDictionary.GetDicomTag(DicomTags.WindowCenter));
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region DicomUid Test
        [Test]
        public void DicomUidTest()
        {
            DicomUid uid = DicomUid.GenerateUid();
        }
        #endregion
    }
}

#endif