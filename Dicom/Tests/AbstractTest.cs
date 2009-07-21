#region License

// Copyright (c) 2009, ClearCanvas Inc.
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

using System.Collections.Generic;

namespace ClearCanvas.Dicom.Tests
{
    public abstract class AbstractTest
    {
        public void SetupMetaInfo(DicomFile theFile)
        {
            DicomAttributeCollection theSet = theFile.MetaInfo;

            theSet[DicomTags.MediaStorageSopClassUid].SetStringValue(theFile.DataSet[DicomTags.SopClassUid].GetString(0, ""));
            theSet[DicomTags.MediaStorageSopInstanceUid].SetStringValue(theFile.DataSet[DicomTags.SopInstanceUid].GetString(0, ""));
            theFile.TransferSyntax = TransferSyntax.ExplicitVrLittleEndian;

            theSet[DicomTags.ImplementationClassUid].SetStringValue("1.1.1.1.1.11.1");
            theSet[DicomTags.ImplementationVersionName].SetStringValue("CC DICOM 1.0");
        }

		public IList<DicomAttributeCollection> SetupMRSeries(int seriesCount, int instancesPerSeries, string studyInstanceUid)
		{
			List<DicomAttributeCollection> instanceList = new List<DicomAttributeCollection>();

			DicomAttributeCollection baseCollection = new DicomAttributeCollection();

			SetupMR(baseCollection);

			baseCollection[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
			
			int acquisitionNumber = 1;
			int instanceNumber = 100;

			float positionX = -61.7564f;
			float positionY = -212.04848f;
			float positionZ = -99.6208f;

			float orientation1 = 0.861f;
			float orientation2 = 0.492f;
			float orientation3 = 0.126f;
			float orientation4 = -0.2965f;


			for (int i = 0; i < seriesCount; i++)
			{
				string seriesInstanceUid = DicomUid.GenerateUid().UID;

				for (int j = 0; j < instancesPerSeries; j++)
				{
					string sopInstanceUid = DicomUid.GenerateUid().UID;
					DicomAttributeCollection instanceCollection = baseCollection.Copy();
					instanceCollection[DicomTags.SopInstanceUid].SetStringValue(sopInstanceUid);
					instanceCollection[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);

					instanceCollection[DicomTags.SeriesNumber].SetStringValue((i + 1).ToString());
					instanceCollection[DicomTags.AcquisitionNumber].SetStringValue(acquisitionNumber++.ToString());
					instanceCollection[DicomTags.InstanceNumber].SetStringValue(instanceNumber++.ToString());

					instanceCollection[DicomTags.ImagePositionPatient].SetFloat32(0, positionX);
					instanceCollection[DicomTags.ImagePositionPatient].SetFloat32(1, positionY);
					instanceCollection[DicomTags.ImagePositionPatient].SetFloat32(2, positionZ);
					positionY += 0.1f;

					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(0, orientation1);
					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(1, orientation2);
					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(2, orientation3);
					instanceCollection[DicomTags.ImageOrientationPatient].SetFloat32(2, orientation4);
					orientation2 += 0.01f;

					instanceList.Add(instanceCollection);
				}
			}

			return instanceList;
		}
        public void SetupMR(DicomAttributeCollection theSet)
        {
            theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
            theSet[DicomTags.ImageType].SetStringValue("ORIGINAL\\PRIMARY\\OTHER\\M\\FFE");
            theSet[DicomTags.InstanceCreationDate].SetStringValue("20070618");
            theSet[DicomTags.InstanceCreationTime].SetStringValue("133600");
            theSet[DicomTags.SopClassUid].SetStringValue(SopClass.MrImageStorageUid);
            theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            theSet[DicomTags.StudyDate].SetStringValue("20070618");
            theSet[DicomTags.StudyTime].SetStringValue("133600");
            theSet[DicomTags.SeriesDate].SetStringValue("20070618");
            theSet[DicomTags.SeriesTime].SetStringValue("133700");
            theSet[DicomTags.AccessionNumber].SetStringValue("A1234");
            theSet[DicomTags.Modality].SetStringValue("MR");
            theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
			theSet[DicomTags.ManufacturersModelName].SetNullValue();
            theSet[DicomTags.InstitutionName].SetStringValue("Mount Sinai Hospital");
            theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
            theSet[DicomTags.StudyDescription].SetStringValue("HEART");
            theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
            theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
            theSet[DicomTags.PatientId].SetStringValue("ID123-45-9999");
            theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
            theSet[DicomTags.PatientsSex].SetStringValue("M");
            theSet[DicomTags.PatientsWeight].SetStringValue("70");
            theSet[DicomTags.SequenceVariant].SetStringValue("OTHER");
            theSet[DicomTags.ScanOptions].SetStringValue("CG");
            theSet[DicomTags.MrAcquisitionType].SetStringValue("2D");
            theSet[DicomTags.SliceThickness].SetStringValue("10.000000");
            theSet[DicomTags.RepetitionTime].SetStringValue("857.142883");
            theSet[DicomTags.EchoTime].SetStringValue("8.712100");
            theSet[DicomTags.NumberOfAverages].SetStringValue("1");
            theSet[DicomTags.ImagingFrequency].SetStringValue("63.901150");
            theSet[DicomTags.ImagedNucleus].SetStringValue("1H");
            theSet[DicomTags.EchoNumbers].SetStringValue("1");
            theSet[DicomTags.MagneticFieldStrength].SetStringValue("1.500000");
            theSet[DicomTags.SpacingBetweenSlices].SetStringValue("10.00000");
            theSet[DicomTags.NumberOfPhaseEncodingSteps].SetStringValue("81");
            theSet[DicomTags.EchoTrainLength].SetStringValue("0");
            theSet[DicomTags.PercentSampling].SetStringValue("63.281250");
            theSet[DicomTags.PercentPhaseFieldOfView].SetStringValue("68.75000");
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
            theSet[DicomTags.InPlanePhaseEncodingDirection].SetStringValue("COL");
            theSet[DicomTags.FlipAngle].SetStringValue("50.000000");
            theSet[DicomTags.PatientPosition].SetStringValue("HFS");
			theSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
			theSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            theSet[DicomTags.StudyId].SetStringValue("1933");
            theSet[DicomTags.SeriesNumber].SetStringValue("1");
            theSet[DicomTags.AcquisitionNumber].SetStringValue("7");
            theSet[DicomTags.InstanceNumber].SetStringValue("1");
            theSet[DicomTags.ImagePositionPatient].SetStringValue("-61.7564\\-212.04848\\-99.6208");
            theSet[DicomTags.ImageOrientationPatient].SetStringValue("0.861\\0.492\\0.126\\-0.2965");
            theSet[DicomTags.FrameOfReferenceUid].SetStringValue(DicomUid.GenerateUid().UID);
            theSet[DicomTags.PositionReferenceIndicator].SetStringValue(null);
            theSet[DicomTags.ImageComments].SetStringValue("Test MR Image");
            theSet[DicomTags.SamplesPerPixel].SetStringValue("1");
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

            DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData);

            byte[] pixelArray = new byte[length];

            for (uint i = 0; i < length; i += 2)
                pixelArray[i] = (byte)(i % 255);

            pixels.Values = pixelArray; 

            theSet[DicomTags.PixelData] = pixels;

            DicomSequenceItem item = new DicomSequenceItem();
            theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

            item[DicomTags.RequestedProcedureId].SetStringValue("MRR1234");
            item[DicomTags.ScheduledProcedureStepId].SetStringValue("MRS1234");

            item = new DicomSequenceItem();
            theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

            item[DicomTags.RequestedProcedureId].SetStringValue("MR2R1234");
            item[DicomTags.ScheduledProcedureStepId].SetStringValue("MR2S1234");

            DicomSequenceItem studyItem = new DicomSequenceItem();

            item[DicomTags.ReferencedStudySequence].AddSequenceItem(studyItem);

            studyItem[DicomTags.ReferencedSopClassUid].SetStringValue(SopClass.MrImageStorageUid);
            studyItem[DicomTags.ReferencedSopInstanceUid].SetStringValue("1.2.3.4.5.6.7.8.9");

        }

        public void SetupMultiframeXA(DicomAttributeCollection theSet, uint rows, uint columns, uint frames)
        {
            theSet[DicomTags.SpecificCharacterSet].SetStringValue("ISO_IR 100");
            theSet[DicomTags.ImageType].SetStringValue("DERIVED\\SECONDARY\\SINGLE PLANE\\SINGLE A");
            theSet[DicomTags.InstanceCreationDate].SetStringValue("20080219");
            theSet[DicomTags.InstanceCreationTime].SetStringValue("143600");
            theSet[DicomTags.SopClassUid].SetStringValue(SopClass.XRayAngiographicImageStorageUid);
            theSet[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            theSet[DicomTags.StudyDate].SetStringValue("20080219");
            theSet[DicomTags.StudyTime].SetStringValue("143600");
            theSet[DicomTags.SeriesDate].SetStringValue("20080219");
            theSet[DicomTags.SeriesTime].SetStringValue("143700");
            theSet[DicomTags.AccessionNumber].SetStringValue("A4567");
            theSet[DicomTags.Modality].SetStringValue("XA");
            theSet[DicomTags.Manufacturer].SetStringValue("ClearCanvas");
            theSet[DicomTags.InstitutionName].SetStringValue("KardiologieUniklinikHeidelberg");
            theSet[DicomTags.ReferringPhysiciansName].SetStringValue("Last^First");
            theSet[DicomTags.StudyDescription].SetStringValue("HEART");
            theSet[DicomTags.SeriesDescription].SetStringValue("Heart 2D EPI BH TRA");
            theSet[DicomTags.PatientsName].SetStringValue("Patient^Test");
            theSet[DicomTags.PatientId].SetStringValue("ID123-45-9999");
            theSet[DicomTags.PatientsBirthDate].SetStringValue("19600101");
            theSet[DicomTags.PatientsSex].SetStringValue("M");
            theSet[DicomTags.PatientsWeight].SetStringValue("80");
            theSet[DicomTags.Kvp].SetStringValue("80");
            theSet[DicomTags.ProtocolName].SetStringValue("25  FPS Koronarien");
            theSet[DicomTags.FrameTime].SetStringValue("40");
            theSet[DicomTags.FrameDelay].SetStringValue("0");
            theSet[DicomTags.DistanceSourceToDetector].SetStringValue("1018");
            theSet[DicomTags.ExposureTime].SetStringValue("7");
            theSet[DicomTags.XRayTubeCurrent].SetStringValue("815");
            theSet[DicomTags.RadiationSetting].SetStringValue("GR");
            theSet[DicomTags.IntensifierSize].SetStringValue("169.99998");
            theSet[DicomTags.PositionerMotion].SetStringValue("STATIC");
            theSet[DicomTags.PositionerPrimaryAngle].SetStringValue("-40.2999999");
            theSet[DicomTags.PositionerSecondaryAngle].SetStringValue("-15.5");
            theSet[DicomTags.DeviceSerialNumber].SetStringValue("1234");
            theSet[DicomTags.SoftwareVersions].SetStringValue("V1.0");
            theSet[DicomTags.StudyInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            theSet[DicomTags.SeriesInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
            theSet[DicomTags.StudyId].SetStringValue("20021208125233");
            theSet[DicomTags.SeriesNumber].SetStringValue("1");
            theSet[DicomTags.AcquisitionNumber].SetStringValue("3");
            theSet[DicomTags.InstanceNumber].SetStringValue("13");
            theSet[DicomTags.PatientOrientation].SetNullValue();
            theSet[DicomTags.ImagesInAcquisition].SetStringValue("1");
            theSet[DicomTags.SamplesPerPixel].SetStringValue("1");
            theSet[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
            theSet[DicomTags.NumberOfFrames].SetUInt32(0, frames);
            theSet[DicomTags.Rows].SetUInt32(0,rows);
            theSet[DicomTags.Columns].SetUInt32(0, columns);
            theSet[DicomTags.FrameIncrementPointer].SetUInt32(0,DicomTags.FrameTime);
            theSet[DicomTags.BitsAllocated].SetStringValue("8");
            theSet[DicomTags.BitsStored].SetStringValue("8");
            theSet[DicomTags.HighBit].SetStringValue("7");
            theSet[DicomTags.PixelRepresentation].SetStringValue("0");
            theSet[DicomTags.WindowCenter].SetStringValue("128");
            theSet[DicomTags.WindowWidth].SetStringValue("204.8");
            theSet[DicomTags.PerformedProcedureStepStartDate].SetStringValue("20080219");
            theSet[DicomTags.PerformedProcedureStepStartTime].SetStringValue("143600");
            theSet[DicomTags.PerformedProcedureStepId].SetStringValue("UNKNOWN");

			// Null SQ Test
			theSet[DicomTags.ReferencedStudySequence].SetNullValue();

			// FL & FD tags for testing
			theSet[DicomTags.SlabThickness].SetFloat64(0,0.1234567d);
			theSet[DicomTags.CalciumScoringMassFactorPatient].SetFloat32(0, 0.7654321f);

            uint length = rows * columns * frames;
            if (length % 2 == 1)
                length++;
            DicomAttributeOW pixels = new DicomAttributeOW(DicomTags.PixelData);

            byte[] pixelArray = new byte[length];
            pixelArray[length - 1] = 0x00; // Padding char

            for (uint frameCount = 0; frameCount < frames; frameCount++)
                for (uint i = frameCount * rows * columns, val = frameCount + 1; i < (frameCount + 1) * rows * columns; i++, val++)
                    pixelArray[i] = (byte)(val % 255);
            
            pixels.Values = pixelArray;

            theSet[DicomTags.PixelData] = pixels;

            DicomSequenceItem item = new DicomSequenceItem();
            theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

            item[DicomTags.RequestedProcedureId].SetStringValue("XA123");
            item[DicomTags.ScheduledProcedureStepId].SetStringValue("XA1234");

            item = new DicomSequenceItem();
            theSet[DicomTags.RequestAttributesSequence].AddSequenceItem(item);

            item[DicomTags.RequestedProcedureId].SetStringValue("XA123");
            item[DicomTags.ScheduledProcedureStepId].SetStringValue("XA1234");


        }
    }
}
