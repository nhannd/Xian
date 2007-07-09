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
            AttributeCollection theSet = new AttributeCollection();
            TestFields theFields = new TestFields();

            SetupMR(theSet);

            theSet.LoadDicomFields(theFields);

            Assert.AreSame(theFields.AccessionNumber, theSet[DicomTags.AccessionNumber].ToString(), "Accession Numbers did not match!");
            Assert.AreSame(theFields.SOPClassUID.UID, theSet[DicomTags.SOPClassUID].ToString(), "SOP Class UIDs did not match!");
            Assert.AreSame(theFields.SOPInstanceUID.UID, theSet[DicomTags.SOPInstanceUID].ToString(), "SOP Class UIDs did not match!");
            Assert.IsTrue(theFields.StudyDate.ToString("yyyyMMdd", CultureInfo.CurrentCulture).Equals(theSet[DicomTags.StudyDate].ToString()));
            Assert.AreSame(theFields.Modality, theSet[DicomTags.Modality].ToString(), "Modality did not match!");
            Assert.AreSame(theFields.StudyDescription, theSet[DicomTags.StudyDescription].ToString(), "Study Description did not match!");
            Assert.AreSame(theFields.StudyInstanceUID.UID, theSet[DicomTags.StudyInstanceUID].ToString(), "Study Instance UIDs did not match!");
            Assert.AreSame(theFields.SeriesInstanceUID.UID, theSet[DicomTags.SeriesInstanceUID].ToString(), "Series Instance UIDs did not match!");
            Assert.AreSame(theFields.StudyID, theSet[DicomTags.StudyID].ToString(), "StudyID did not match!");
            Assert.AreSame(theFields.PatientsName, theSet[DicomTags.PatientsName].ToString(), "PatientsName did not match!");
            Assert.AreSame(theFields.PatientID, theSet[DicomTags.PatientID].ToString(), "PatientID did not match!");
            Assert.IsTrue(theFields.PatientsBirthDate.ToString("yyyyMMdd", CultureInfo.CurrentCulture).Equals(theSet[DicomTags.PatientsBirthDate].ToString()));
            Assert.AreSame(theFields.PatientsSex, theSet[DicomTags.PatientsSex].ToString(), "PatientsSex did not match!");
            Assert.IsTrue(theFields.Rows == theSet[DicomTags.Rows].GetUInt16(0));
            Assert.IsTrue(theFields.Columns == theSet[DicomTags.Columns].GetUInt16(0));
            Assert.IsTrue(theFields.PixelSpacing == theSet[DicomTags.PixelSpacing].GetFloat32(0));
            Assert.IsTrue(theFields.InstanceNumber == theSet[DicomTags.InstanceNumber].GetInt32(0));
            //Assert.IsTrue(string.Join("\\", theFields.ImageType).Equals(theSet[DicomTags.ImageType].ToString()));
        }

    }
}

#endif