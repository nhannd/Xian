using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Dicom.Codec;
using NUnit.Framework;

namespace ClearCanvas.Dicom.Tests
{
    [TestFixture]
    public class CodecTest : AbstractTest
    {
        [Test]
        public void RleTest()
        {
            DicomFile file = new DicomFile("RleCodecTest.dcm");

            SetupMR(file.DataSet);

            SetupMetaInfo(file);

            RleTest(file);

            file = new DicomFile("MultiframeRleCodecTest.dcm");

            this.SetupMultiframeXA(file.DataSet, 511, 511, 5);

            RleTest(file);
        }


        public void RleTest(DicomFile file)
        {
            // Make a copy of the source format
            DicomFile originalFile;
            DicomAttributeCollection originalDataSet = file.DataSet.Copy();
            DicomAttributeCollection originalMetaInfo = file.MetaInfo.Copy();
            originalFile = new DicomFile("", originalMetaInfo, originalDataSet);

            IDicomCodec rleCodec = new DicomRleCodec();

            DicomUncompressedPixelData pd = new DicomUncompressedPixelData(file.DataSet);
            DicomCompressedPixelData fragments = new DicomCompressedPixelData(pd);

            rleCodec.Encode(file.DataSet, pd, fragments, null);

            fragments.TransferSyntax = TransferSyntax.RleLossless;

            fragments.UpdateMessage(file);

            file.Save();

            DicomFile newFile = new DicomFile(file.Filename);

            newFile.Load();
            fragments = new DicomCompressedPixelData(newFile.DataSet);
            pd = new DicomUncompressedPixelData(fragments);

            rleCodec.Decode(newFile.DataSet, fragments, pd, null);

            pd.UpdateMessage(newFile);

            newFile.Filename = "Output" + file.Filename;
            newFile.Save();

            Assert.AreEqual(originalFile.DataSet.Equals(newFile.DataSet), true);
        }
    }
}
