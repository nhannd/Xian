#if UNIT_TESTS
using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using ClearCanvas.Dicom;

namespace ClearCanvas.Dicom.Tests
{
    [TestFixture]
    public class FileTest : AbstractTest
    {
        [Test]
        public void ConstructorTests()
        {
            DicomFile file = new DicomFile(null);

            file = new DicomFile("filename");

            file = new DicomFile(null, new DicomAttributeCollection(), new DicomAttributeCollection());


        }

        private void SetupMetaInfo(DicomFile theFile)
        {
            DicomAttributeCollection theSet = theFile.MetaInfo;

            theSet[DicomTags.MediaStorageSopClassUid].SetStringValue(theFile.DataSet[DicomTags.SopClassUid].GetString(0,""));
            theSet[DicomTags.MediaStorageSopInstanceUid].SetStringValue(theFile.DataSet[DicomTags.SopInstanceUid].GetString(0, ""));
            theFile.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian; 

            theSet[DicomTags.ImplementationClassUid].SetStringValue("1.1.1.1.1.11.1");
            theSet[DicomTags.ImplementationVersionName].SetStringValue("CC ImageServer 1.0");
        }


        public void WriteOptionsTest(DicomFile sourceFile, DicomWriteOptions options)
        {
            bool result = sourceFile.Save(options);

            Assert.AreEqual(result, true);

            DicomFile newFile = new DicomFile("CreateFileTest.dcm");

            DicomReadOptions readOptions = DicomReadOptions.Default;
            newFile.Load(readOptions);

            Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);

            // update a tag, and make sure it shows they're different
            newFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
            Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), false);

            // Now update the original file with the name, and they should be the same again
            sourceFile.DataSet[DicomTags.PatientsName].Values = "NewPatient^First";
            Assert.AreEqual(sourceFile.DataSet.Equals(newFile.DataSet), true);

            // Return the original string value.
            sourceFile.DataSet[DicomTags.PatientsName].SetStringValue("Patient^Test");

        }

        [Test]
        public void CreateFileTest()
        {
            DicomFile file = new DicomFile("CreateFileTest.dcm");

            DicomAttributeCollection dataSet = file.DataSet;

            DicomAttributeCollection metaInfo = file.DataSet;


            SetupMR(dataSet);

            SetupMetaInfo(file);

            DicomWriteOptions writeOptions = DicomWriteOptions.Default;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.ExplicitLengthSequence;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.ExplicitLengthSequenceItem;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.ExplicitLengthSequence | DicomWriteOptions.ExplicitLengthSequenceItem;
            WriteOptionsTest(file, writeOptions);

            writeOptions = DicomWriteOptions.None;
            WriteOptionsTest(file, writeOptions);

        }
    }
}
#endif