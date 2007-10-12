#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.IO;
using NUnit.Framework;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Tests
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
                DicomAttributeAE attrib = new DicomAttributeAE(DicomTagDictionary.GetDicomTag(DicomTags.RetrieveAeTitle));
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

        #region DicomAttributeAT Test
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
        #endregion

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

        #region DicomAttributeOW Test
        [Test]
        public void AttributeOWTest()
        {
            DicomAttributeOW attrib = new DicomAttributeOW(DicomTags.PixelData);

            uint length = 256 * 256 * 2;
            
            byte[] pixelArray = new byte[length];

            for (uint i = 0; i < length; i += 2)
                pixelArray[i] = (byte)(i % 255);

            attrib.Values = pixelArray;

            // big endian test
            ByteBuffer bb = attrib.GetByteBuffer(TransferSyntax.ExplicitVrBigEndian, null);

            DicomAttributeOW bigEndAttrib = new DicomAttributeOW(DicomTagDictionary.GetDicomTag(DicomTags.PixelData), bb);

            byte[] bigEndArray = (byte[])bigEndAttrib.Values;

            for (uint i = 0; i < length; i++)
            {
                Assert.AreEqual(pixelArray[i], bigEndArray[i]);
            }

            // little endian test
            bb = attrib.GetByteBuffer(TransferSyntax.ExplicitVrLittleEndian, null);

            DicomAttributeOW littleEndAttrib = new DicomAttributeOW(DicomTagDictionary.GetDicomTag(DicomTags.PixelData), bb);

            byte[] littleEndArray = (byte[])littleEndAttrib.Values;

            for (uint i = 0; i < length; i++)
            {
                Assert.AreEqual(pixelArray[i], littleEndArray[i]);
            }

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
            DicomUid uid2 = DicomUid.GenerateUid();
            Assert.AreNotEqual(uid.UID, uid2.UID);
        }
        #endregion
    }
}

#endif