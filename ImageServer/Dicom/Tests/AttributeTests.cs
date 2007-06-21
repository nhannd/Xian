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
    public class AttributeTests
    {
        #region AttributeAE Test
        [Test]
        public void AttributeAETest()
        {
            bool testResult = false;
            try
            {
                AttributeAE attrib = new AttributeAE(DicomTagDictionary.Instance[DicomTags.AccessionNumber]);
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                AttributeAE attrib = new AttributeAE(DicomTagDictionary.Instance[DicomTags.RetrieveAETitle]);
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);

        }
        #endregion

        #region AttributeAS Test
        [Test]
        public void AttributeASTest()
        {
            bool testResult = false;
            try
            {
                AttributeAS attrib = new AttributeAS(DicomTagDictionary.Instance[DicomTags.AccessionNumber]);
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                AttributeAS attrib = new AttributeAS(DicomTagDictionary.Instance[DicomTags.PatientsAge]);
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
                AttributeAT attrib = new AttributeAT(DicomTagDictionary.Instance[DicomTags.AccessionNumber]);
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                AttributeAT attrib = new AttributeAT(DicomTagDictionary.Instance[DicomTags.FrameIncrementPointer]);
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }

        #region AttributeCS Test
        [Test]
        public void AttributeCSTest()
        {
            bool testResult = false;
            try
            {
                AttributeCS attrib = new AttributeCS(DicomTagDictionary.Instance[DicomTags.AccessionNumber]);
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                AttributeCS attrib = new AttributeCS(DicomTagDictionary.Instance[DicomTags.ImageType]);
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region AttributeDA Test
        [Test]
        public void AttributeDATest()
        {
            bool testResult = false;
            try
            {
                AttributeDA attrib = new AttributeDA(DicomTagDictionary.Instance[DicomTags.AccessionNumber]);
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                AttributeDA attrib = new AttributeDA(DicomTagDictionary.Instance[DicomTags.StudyDate]);
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion

        #region AttributeDS Test
        [Test]
        public void AttributeDSTest()
        {
            bool testResult = false;
            try
            {
                AttributeDS attrib = new AttributeDS(DicomTagDictionary.Instance[DicomTags.AccessionNumber]);
            }
            catch (DicomException)
            {
                testResult = true;
            }
            Assert.AreEqual(testResult, true);

            testResult = true;
            try
            {
                AttributeDS attrib = new AttributeDS(DicomTagDictionary.Instance[DicomTags.WindowCenter]);
                testResult = true;
            }
            catch (DicomException)
            {
                testResult = false;
            }
            Assert.AreEqual(testResult, true);


        }
        #endregion


    }
}

#endif