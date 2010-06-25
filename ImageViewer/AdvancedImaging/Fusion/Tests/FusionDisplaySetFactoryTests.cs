#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Tests
{
	[TestFixture]
	public class FusionDisplaySetFactoryTests
	{
		[Test]
		public void TestNullCreation()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var displaySets = CreateDisplaySets(factory, Combine<ISopDataSource>());

			Assert.IsEmpty(displaySets);
		}

		[Test]
		public void TestPETAndCTFusion()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			Assert.IsNotEmpty(displaySets);
		}

		[Test]
		public void TestNoFusingWrongModality()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesMR = CreateSopSeries(25, "PatientA", "StudyA", "SeriesMR", 1, "FrameA", Modality.MR);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyA", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesMR, seriesPET));

			Assert.IsEmpty(displaySets);
		}

		[Test]
		public void TestNoFusingInvalidModalities()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesMR = CreateSopSeries(25, "PatientA", "StudyA", "SeriesMR", 1, "FrameA", Modality.MR);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesMR));

			Assert.IsEmpty(displaySets);
		}

		[Test]
		public void TestNoFusingDifferentStudies()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientA", "StudyB", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			Assert.IsEmpty(displaySets);
		}

		[Test]
		public void TestNoFusingDifferentPatients()
		{
			var factory = new PETFusionDisplaySetFactory(PETFusionType.CT);
			var seriesCT = CreateSopSeries(25, "PatientA", "StudyA", "SeriesCT", 1, "FrameA", Modality.CT);
			var seriesPET = CreateSopSeries(25, "PatientB", "StudyB", "SeriesPET", 2, "FrameA", Modality.PT);
			var displaySets = CreateDisplaySets(factory, Combine(seriesCT, seriesPET));

			Assert.IsEmpty(displaySets);
		}

		private static List<IDisplaySet> CreateDisplaySets(IDisplaySetFactory displaySetFactory, IEnumerable<ISopDataSource> sopDataSources)
		{
			var studyTree = new StudyTree();
			foreach (var sopDataSource in sopDataSources)
			{
				studyTree.AddSop(new ImageSop(sopDataSource));
			}
			displaySetFactory.SetStudyTree(studyTree);

			var displaySets = new List<IDisplaySet>();
			foreach (var patient in studyTree.Patients)
			{
				foreach (var study in patient.Studies)
				{
					foreach (var series in study.Series)
					{
						displaySets.AddRange(displaySetFactory.CreateDisplaySets(series));
					}
				}
			}
			return displaySets;
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount, string patientId, string studyId, string seriesDesc, int seriesNumber, string frameOfReferenceId, Modality modality)
		{
			return CreateSopSeries(sopCount, patientId, patientId, studyId, HashUid(studyId), seriesDesc, seriesNumber, HashUid(frameOfReferenceId), modality);
		}

		private static IEnumerable<ISopDataSource> CreateSopSeries(int sopCount,
		                                                           string patientId, string patientName,
		                                                           string studyId, string studyInstanceUid,
		                                                           string seriesDesc, int seriesNumber,
		                                                           string frameOfReferenceUid, Modality modality)
		{
			string seriesInstanceUid = DicomUid.GenerateUid().UID;
			for (int n = 0; n < sopCount; n++)
			{
				var dicomFile = new DicomFile();
				var dataset = dicomFile.DataSet;
				dataset[DicomTags.PatientId].SetStringValue(patientId);
				dataset[DicomTags.PatientsName].SetStringValue(patientName);
				dataset[DicomTags.StudyId].SetStringValue(studyId);
				dataset[DicomTags.StudyInstanceUid].SetStringValue(studyInstanceUid);
				dataset[DicomTags.SeriesDescription].SetStringValue(seriesDesc);
				dataset[DicomTags.SeriesNumber].SetInt32(0, seriesNumber);
				dataset[DicomTags.SeriesInstanceUid].SetStringValue(seriesInstanceUid);
				dataset[DicomTags.SopInstanceUid].SetStringValue(DicomUid.GenerateUid().UID);
				dataset[DicomTags.SopClassUid].SetStringValue(ModalityConverter.ToSopClassUid(modality));
				dataset[DicomTags.Modality].SetStringValue(modality.ToString());
				dataset[DicomTags.FrameOfReferenceUid].SetStringValue(frameOfReferenceUid);
				dataset[DicomTags.ImageOrientationPatient].SetStringValue(string.Format(@"{0}\{1}\{2}\{3}\{4}\{5}", 1, 0, 0, 0, 1, 0));
				dataset[DicomTags.ImagePositionPatient].SetStringValue(string.Format(@"{0}\{1}\{2}", 0, 0, n));
				dataset[DicomTags.PixelSpacing].SetStringValue(string.Format(@"{0}\{1}", 0.5, 0.5));
				dataset[DicomTags.PhotometricInterpretation].SetStringValue("MONOCHROME2");
				dataset[DicomTags.SamplesPerPixel].SetInt32(0, 1);
				dataset[DicomTags.BitsStored].SetInt32(0, 16);
				dataset[DicomTags.BitsAllocated].SetInt32(0, 16);
				dataset[DicomTags.HighBit].SetInt32(0, 15);
				dataset[DicomTags.PixelRepresentation].SetInt32(0, 1);
				dataset[DicomTags.Rows].SetInt32(0, 100);
				dataset[DicomTags.Columns].SetInt32(0, 100);
				dataset[DicomTags.WindowCenter].SetInt32(0, 0);
				dataset[DicomTags.WindowWidth].SetInt32(0, 65536);
				dataset[DicomTags.WindowCenterWidthExplanation].SetString(0, "Full Window");
				dataset[DicomTags.PixelData].Values = new byte[2*100*100];
				dicomFile.MediaStorageSopClassUid = dataset[DicomTags.SopClassUid];
				dicomFile.MediaStorageSopInstanceUid = dataset[DicomTags.SopInstanceUid];
				yield return new XSopDataSource(dicomFile);
			}
		}

		private static string HashUid(string id)
		{
			return string.Format("411.12.8453.12.83109.70.5.{0}", id.GetHashCode());
		}

		private static IEnumerable<T> Combine<T>(params IEnumerable<T>[] enumerables)
		{
			if (enumerables == null)
				yield break;

			foreach (var enumerable in enumerables)
				foreach (var item in enumerable)
					yield return item;
		}

		private class XSopDataSource : DicomMessageSopDataSource
		{
			public XSopDataSource(DicomMessageBase sourceMessage) : base(sourceMessage) {}
		}
	}
}

#endif