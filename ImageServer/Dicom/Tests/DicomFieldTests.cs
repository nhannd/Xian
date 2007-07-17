#if UNIT_TESTS

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

using NUnit.Framework;
using ClearCanvas.ImageServer.Dicom;
using ClearCanvas.ImageServer.Dicom.Exceptions;

namespace ClearCanvas.ImageServer.Dicom.Tests
{
    [TestFixture]
    public class DicomFieldTests : AbstractTest
    {
        public class TestFields
        {
            [DicomField(DicomTags.SOPClassUID, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid SOPClassUID = null;

            [DicomField(DicomTags.SOPInstanceUID, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid SOPInstanceUID = null;

            [DicomField(DicomTags.StudyDate, DefaultValue = DicomFieldDefault.Default)]
            public DateTime StudyDate;

            [DicomField(DicomTags.AccessionNumber, DefaultValue = DicomFieldDefault.Default)]
            public string AccessionNumber = null;

            [DicomField(DicomTags.Modality, DefaultValue = DicomFieldDefault.Default)]
            public string Modality = null;

            [DicomField(DicomTags.StudyDescription, DefaultValue = DicomFieldDefault.Default)]
            public string StudyDescription = null;

            [DicomField(DicomTags.StudyInstanceUID, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid StudyInstanceUID = null;

            [DicomField(DicomTags.SeriesInstanceUID, DefaultValue = DicomFieldDefault.Default)]
            public DicomUid SeriesInstanceUID = null;

            [DicomField(DicomTags.StudyID, DefaultValue = DicomFieldDefault.Default)]
            public string StudyID = null;

            [DicomField(DicomTags.PatientsName, DefaultValue = DicomFieldDefault.Default)]
            public string PatientsName = null;

            [DicomField(DicomTags.PatientID, DefaultValue = DicomFieldDefault.Default)]
            public string PatientID = null;

            [DicomField(DicomTags.PatientsBirthDate, DefaultValue = DicomFieldDefault.Default)]
            public DateTime PatientsBirthDate;

            [DicomField(DicomTags.PatientsSex, DefaultValue = DicomFieldDefault.Default)]
            public string PatientsSex = null;

            [DicomField(DicomTags.Rows, DefaultValue = DicomFieldDefault.Default)]
            public ushort Rows = 0;

            [DicomField(DicomTags.Columns, DefaultValue = DicomFieldDefault.Default)]
            public ushort Columns = 0;

            [DicomField(DicomTags.PixelSpacing, DefaultValue = DicomFieldDefault.Default)]
            public float PixelSpacing = 0.0f;

            [DicomField(DicomTags.InstanceNumber, DefaultValue = DicomFieldDefault.Default)]
            public int InstanceNumber = 0;

            [DicomField(DicomTags.ImageType, DefaultValue = DicomFieldDefault.Default)]
            public string[] ImageType = null;

            [DicomField(DicomTags.ImagePositionPatient, DefaultValue = DicomFieldDefault.Default)]
            public float[] ImagePositionPatient = null;
            
        }

        [Test]
        public void FieldTest()
        {
            DicomAttributeCollection theSet = new DicomAttributeCollection();
            TestFields theFields = new TestFields();

            SetupMR(theSet);

            theSet.LoadDicomFields(theFields);

            Assert.IsTrue(theFields.AccessionNumber.Equals(theSet[DicomTags.AccessionNumber].GetString(0,"")), "Accession Numbers did not match!");
            Assert.IsTrue(theFields.SOPClassUID.UID.Equals(theSet[DicomTags.SOPClassUID].GetString(0, "")), "SOP Class UIDs did not match!");
            Assert.IsTrue(theFields.SOPInstanceUID.UID.Equals(theSet[DicomTags.SOPInstanceUID].GetString(0, "")), "SOP Class UIDs did not match!");
            Assert.IsTrue(theFields.StudyDate.ToString("yyyyMMdd", CultureInfo.CurrentCulture).Equals(theSet[DicomTags.StudyDate].GetString(0, "")));
            Assert.IsTrue(theFields.Modality.Equals(theSet[DicomTags.Modality].GetString(0, "")), "Modality did not match!");
            Assert.IsTrue(theFields.StudyDescription.Equals(theSet[DicomTags.StudyDescription].GetString(0, "")), "Study Description did not match!");
            Assert.IsTrue(theFields.StudyInstanceUID.UID.Equals(theSet[DicomTags.StudyInstanceUID].GetString(0, "")), "Study Instance UIDs did not match!");
            Assert.IsTrue(theFields.SeriesInstanceUID.UID.Equals(theSet[DicomTags.SeriesInstanceUID].GetString(0, "")), "Series Instance UIDs did not match!");
            Assert.IsTrue(theFields.StudyID.Equals(theSet[DicomTags.StudyID].GetString(0, "")), "StudyID did not match!");
            Assert.IsTrue(theFields.PatientsName.Equals(theSet[DicomTags.PatientsName].GetString(0, "")), "PatientsName did not match!");
            Assert.IsTrue(theFields.PatientID.Equals(theSet[DicomTags.PatientID].GetString(0, "")), "PatientID did not match!");
            Assert.IsTrue(theFields.PatientsBirthDate.ToString("yyyyMMdd", CultureInfo.CurrentCulture).Equals(theSet[DicomTags.PatientsBirthDate].GetString(0, "")));
            Assert.IsTrue(theFields.PatientsSex.Equals(theSet[DicomTags.PatientsSex].GetString(0, "")), "PatientsSex did not match!");
            Assert.IsTrue(theFields.Rows == theSet[DicomTags.Rows].GetUInt16(0,0));
            Assert.IsTrue(theFields.Columns == theSet[DicomTags.Columns].GetUInt16(0,0));
            float floatValue;
            theSet[DicomTags.PixelSpacing].TryGetFloat32(0, out floatValue);
            Assert.IsTrue(theFields.PixelSpacing == floatValue);
            int intValue;
            theSet[DicomTags.InstanceNumber].TryGetInt32(0, out intValue);
            Assert.IsTrue(theFields.InstanceNumber == intValue);
            //Assert.IsTrue(string.Join("\\", theFields.ImageType).Equals(theSet[DicomTags.ImageType].ToString()));
        }

    }
}

#endif