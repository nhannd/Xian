using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Tests
{
    public abstract class AbstractTest
    {
        public void SetupMR(DicomAttributeCollection theSet)
        {
            theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
            theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
            theSet[DicomTags.InstanceCreationDate].SetStringValue("20070618");
            theSet[DicomTags.InstanceCreationTime].SetStringValue("133600");
            theSet[DicomTags.SOPClassUID].SetStringValue(SopClass.MRImageStorageUid);
            theSet[DicomTags.SOPInstanceUID].SetStringValue("1.1.1.1.1");
            theSet[DicomTags.StudyDate].SetStringValue("20070618");
            theSet[DicomTags.StudyTime].SetStringValue("1336000");
            theSet[DicomTags.SeriesDate].SetStringValue("20070618");
            theSet[DicomTags.SeriesTime].SetStringValue("133700");
            theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
            theSet[DicomTags.Modality].SetStringValue("MR");
            theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
            theSet[DicomTags.InstitutionName].SetStringValue("Mount Sinai Hospital");
            theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
            theSet[DicomTags.StudyDescription].SetStringValue("HEART");
            theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
            theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
            theSet[DicomTags.PatientID].SetStringValue("ID123-45-9999");
            theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
            theSet[DicomTags.PatientsSex].SetStringValue("M");
            theSet[DicomTags.PatientsWeight].SetStringValue("70");
            theSet[DicomTags.SequenceVariant].SetStringValue("OTHER");
            theSet[DicomTags.ScanOptions].SetStringValue("CG");
            theSet[DicomTags.MRAcquisitionType].SetStringValue("2D");
            theSet[DicomTags.SliceThickness].SetStringValue("10.000000");
            theSet[DicomTags.RepetitionTime].SetStringValue("857.142883");
            theSet[DicomTags.EchoTime].SetStringValue("8.712100");
            theSet[DicomTags.NumberofAverages].SetStringValue("1");
            theSet[DicomTags.ImagingFrequency].SetStringValue("63.901150");
            theSet[DicomTags.ImagedNucleus].SetStringValue("1H");
            theSet[DicomTags.EchoNumbers].SetStringValue("1");
            theSet[DicomTags.MagneticFieldStrength].SetStringValue("1.500000");
            theSet[DicomTags.SpacingBetweenSlices].SetStringValue("10.00000");
            theSet[DicomTags.NumberofPhaseEncodingSteps].SetStringValue("81");
            theSet[DicomTags.EchoTrainLength].SetStringValue("0");
            theSet[DicomTags.PercentSampling].SetStringValue("63.281250");
            theSet[DicomTags.PercentPhaseFieldofView].SetStringValue("68.75000");
            theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
            theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
            theSet[DicomTags.ProtocolName].SetStringValue("2D EPI BH");
            theSet[DicomTags.TriggerTime].SetStringValue("14.000000");
            theSet[DicomTags.LowRRValue].SetStringValue("948");
            theSet[DicomTags.HighRRValue].SetStringValue("1178");
            theSet[DicomTags.IntervalsAcquired].SetStringValue("102");
            theSet[DicomTags.IntervalsRejected].SetStringValue("0");
            theSet[DicomTags.HeartRate].SetStringValue("56");
            theSet[DicomTags.ReceiveCoilName].SetStringValue("B");
            theSet[DicomTags.TransmitCoilName].SetStringValue("B");
            theSet[DicomTags.InplanePhaseEncodingDirection].SetStringValue("COL");
            theSet[DicomTags.FlipAngle].SetStringValue("50.000000");
            theSet[DicomTags.PatientPosition].SetStringValue("HFS");
            theSet[DicomTags.StudyInstanceUID].SetStringValue("1.1.1.1.1.2");
            theSet[DicomTags.SeriesInstanceUID].SetStringValue("1.1.1.1.1.3");
            theSet[DicomTags.StudyID].SetStringValue("1933");
            theSet[DicomTags.SeriesNumber].SetStringValue("1");
            theSet[DicomTags.AcquisitionNumber].SetStringValue("7");
            theSet[DicomTags.InstanceNumber].SetStringValue("1");
            theSet[DicomTags.ImagePositionPatient].SetStringValue("-61.7564\\-212.04848\\-99.6208");
            theSet[DicomTags.ImageOrientationPatient].SetStringValue("0.861\\0.492\\0.126\\-0.2965");
            theSet[DicomTags.FrameofReferenceUID].SetStringValue("1.1.1.1.1.4");
            theSet[DicomTags.PositionReferenceIndicator].SetStringValue(null);
            theSet[DicomTags.ImageComments].SetStringValue("Test MR Image");
            theSet[DicomTags.SamplesperPixel].SetStringValue("1");
            theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
            theSet[DicomTags.Rows].SetStringValue("256");
            theSet[DicomTags.Columns].SetStringValue("256");
            theSet[DicomTags.PixelSpacing].SetStringValue("1.367188");
            theSet[DicomTags.BitsAllocated].SetStringValue("16");
            theSet[DicomTags.BitsStored].SetStringValue("12");
            theSet[DicomTags.HighBit].SetStringValue("11");
            theSet[DicomTags.PixelRepresentation].SetStringValue("0");
            theSet[DicomTags.WindowCenter].SetStringValue("238");
            theSet[DicomTags.WindowWidth].SetStringValue("471");

            uint length = 256 * 256 * 2;

            DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData); ;

            byte[] pixelArray = new byte[length];

            for (uint i = 0; i < length; i += 2)
                pixelArray[i] = (byte)(i % 255);

            pixels.Values = pixelArray;

            theSet[DicomTags.PixelData] = pixels;

            DicomSequenceItem item = new DicomSequenceItem();
            theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

            item[DicomTags.RequestedProcedureID].SetStringValue("MRR1234");
            item[DicomTags.ScheduledProcedureStepID].SetStringValue("MRS1234");

            item = new DicomSequenceItem();
            theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

            item[DicomTags.RequestedProcedureID].SetStringValue("MR2R1234");
            item[DicomTags.ScheduledProcedureStepID].SetStringValue("MR2S1234");

            DicomSequenceItem studyItem = new DicomSequenceItem();

            item[DicomTags.ReferencedStudySequence].AddSequenceItem(studyItem);

            studyItem[DicomTags.ReferencedSOPClassUID].SetStringValue(SopClass.MRImageStorageUid);
            studyItem[DicomTags.ReferencedSOPInstanceUID].SetStringValue("1.2.3.4.5.6.7.8.9");

        }
    }
}
