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

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.Utilities;
using ClearCanvas.ImageViewer.AdvancedImaging.Fusion.Utilities;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.AdvancedImaging.Fusion
{
	public class PETFusionDisplaySetFactory : DisplaySetFactory
	{
		private const float _halfPi = (float) Math.PI/2;
		private const float _gantryTiltTolerance = 0.1f; // allowed tolerance for gantry tilt (in radians)
		private const float _orientationTolerance = 0.01f; // allowed tolerance for image orientation (direction cosine values)
		private const float _minimumSliceSpacing = 0.01f; // minimum spacing required between slices (in mm)
		private const float _sliceSpacingTolerance = 0.001f; // allowed tolerance for slice spacing (in mm)
		private const string _attenuationCorrectionCode = "ATTN";
		private const string _petModality = "PT";

		private readonly PETFusionType _fusionType;

		public PETFusionDisplaySetFactory(PETFusionType fusionType)
		{
			_fusionType = fusionType;
		}

		public PETFusionType FusionType
		{
			get { return _fusionType; }
		}

		private static bool IsAttenuationCorrected(Sop sop)
		{
			var correctionTypes = DicomStringHelper.GetStringArray(sop[DicomTags.CorrectedImage].ToString().ToUpperInvariant());
			return (Array.FindIndex(correctionTypes, s => s == _attenuationCorrectionCode) >= 0);
		}

		public override List<IDisplaySet> CreateDisplaySets(Series series)
		{
			List<IDisplaySet> displaySets = new List<IDisplaySet>();
			if (this.IsValidBaseSeries(series))
			{
				foreach (var ptSeries in FindFuseablePETSeries(series))
				{
					if (ptSeries.Sops.Count == 0)
						continue;

					var descriptor = new PETFusionDisplaySetDescriptor(series.GetIdentifier(), ptSeries.GetIdentifier(), IsAttenuationCorrected(ptSeries.Sops[0]));
					var displaySet = new DisplaySet(descriptor);
					using (var fusionOverlayData = new FusionOverlayData(GetFrames(ptSeries.Sops)))
					{
						foreach (var baseFrame in GetFrames(series.Sops))
						{
							using (var fusionOverlaySlice = new FusionOverlaySlice(baseFrame, fusionOverlayData))
							{
								var fus = new FusionPresentationImage(baseFrame, fusionOverlaySlice);
								displaySet.PresentationImages.Add(fus);
							}
						}
					}
					displaySets.Add(displaySet);
				}
			}
			return displaySets;
		}

		public bool IsValidBaseSeries(Series series)
		{
			switch (_fusionType)
			{
				case PETFusionType.CT:
					if (series.Modality != _fusionType.ToString())
						return false;

					var frames = GetFrames(series.Sops);
					if (frames.Count < 5)
						return false;
					if (!AssertSameSeriesFrameOfReference(frames))
						return false;
					foreach (Frame frame in frames)
					{
						if (frame.ImageOrientationPatient.IsNull)
							return false;
						if (frame.NormalizedPixelSpacing.IsNull)
							return false;
					}
					return true;
			}
			return false;
		}

		public bool IsValidPETFusionSeries(Series series)
		{
			if (series.Modality != _petModality)
				return false;

			var frames = GetFrames(series.Sops);
			if (frames.Count < 5)
				return false;
			if (!AssertSameSeriesFrameOfReference(frames))
				return false;

			// ensure all frames have the same orientation
			ImageOrientationPatient orient = frames[0].ImageOrientationPatient;
			double minColumnSpacing = double.MaxValue, minRowSpacing = double.MaxValue;
			double maxColumnSpacing = double.MinValue, maxRowSpacing = double.MinValue;
			foreach (Frame frame in frames)
			{
				if (frame.ImageOrientationPatient.IsNull)
					return false;
				if (!frame.ImageOrientationPatient.EqualsWithinTolerance(orient, _orientationTolerance))
					return false;

				PixelSpacing pixelSpacing = frame.NormalizedPixelSpacing;
				if (pixelSpacing.IsNull)
					return false;

				minColumnSpacing = Math.Min(minColumnSpacing, pixelSpacing.Column);
				maxColumnSpacing = Math.Max(maxColumnSpacing, pixelSpacing.Column);
				minRowSpacing = Math.Min(minRowSpacing, pixelSpacing.Row);
				maxRowSpacing = Math.Max(maxRowSpacing, pixelSpacing.Row);
			}

			// ensure all frames have consistent pixel spacing
			if (maxColumnSpacing - minColumnSpacing > _sliceSpacingTolerance || maxRowSpacing - minRowSpacing > _sliceSpacingTolerance)
				return false;

			// ensure all frames are sorted by slice location
			frames.Sort(new SliceLocationComparer().Compare);

			// ensure all frames are equally spaced
			float? nominalSpacing = null;
			for (int i = 1; i < frames.Count; i++)
			{
				float currentSpacing = CalcSpaceBetweenPlanes(frames[i], frames[i - 1]);
				if (currentSpacing < _minimumSliceSpacing)
					return false;
				if (!nominalSpacing.HasValue)
					nominalSpacing = currentSpacing;
				if (!FloatComparer.AreEqual(currentSpacing, nominalSpacing.Value, _sliceSpacingTolerance))
					return false;
			}

			// ensure frames are not tilted about unsupposed axis combinations (the gantry correction algorithm only supports rotations about X)
			if (!IsSupportedGantryTilt(frames)) // suffices to check first one... they're all co-planar now!!
				return false;

			return true;
		}

		public bool CanFuse(Series baseSeries, Series petSeries)
		{
			if (!IsValidBaseSeries(baseSeries))
				return false;
			if (!IsValidPETFusionSeries(petSeries))
				return false;

			//var baseFrames = GetFrames(baseSeries.Sops);
			//var petFrames = GetFrames(petSeries.Sops);
			//if (baseFrames[0].FrameOfReferenceUid != petFrames[0].FrameOfReferenceUid)
			//	return false;

			return true;
		}

		private IEnumerable<Series> FindFuseablePETSeries(Series baseSeries)
		{
			var baseStudy = baseSeries.ParentStudy;
			if (baseStudy == null)
				yield break;

			foreach (var series in baseStudy.Series)
			{
				if (CanFuse(baseSeries, series))
					yield return series;
			}
		}

		private static bool AssertSameSeriesFrameOfReference(IList<Frame> frames)
		{
			// ensure all frames have are from the same series, and have the same frame of reference
			string studyInstanceUid = frames[0].StudyInstanceUid;
			string seriesInstanceUid = frames[0].SeriesInstanceUid;
			string frameOfReferenceUid = frames[0].FrameOfReferenceUid;
			foreach (Frame frame in frames)
			{
				if (frame.StudyInstanceUid != studyInstanceUid)
					return false;
				if (frame.SeriesInstanceUid != seriesInstanceUid)
					return false;
				if (frame.FrameOfReferenceUid != frameOfReferenceUid)
					return false;
			}
			return true;
		}

		private static List<Frame> GetFrames(IEnumerable<Sop> sops)
		{
			List<Frame> list = new List<Frame>();
			foreach (var sop in sops)
				if (sop is ImageSop)
					list.AddRange(((ImageSop) sop).Frames);
			return list;
		}

		private static float CalcSpaceBetweenPlanes(Frame frame1, Frame frame2)
		{
			Vector3D point1 = frame1.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			Vector3D point2 = frame2.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			Vector3D delta = point1 - point2;

			return delta.IsNull ? 0f : delta.Magnitude;
		}

		private static bool IsSupportedGantryTilt(IList<Frame> frames)
		{
			try
			{
				using (IPresentationImage firstImage = ImageViewer.PresentationImageFactory.Create(frames[0]))
				{
					using (IPresentationImage lastImage = ImageViewer.PresentationImageFactory.Create(frames[frames.Count - 1]))
					{
						// neither of these should return null since we already checked for image orientation and position (patient)
						DicomImagePlane firstImagePlane = DicomImagePlane.FromImage(firstImage);
						DicomImagePlane lastImagePlane = DicomImagePlane.FromImage(lastImage);

						Vector3D stackZ = lastImagePlane.PositionPatientTopLeft - firstImagePlane.PositionPatientTopLeft;
						Vector3D imageX = firstImagePlane.PositionPatientTopRight - firstImagePlane.PositionPatientTopLeft;

						if (!stackZ.IsOrthogonalTo(imageX, _gantryTiltTolerance))
						{
							// this is a gantry slew (gantry tilt about Y axis)
							return false;
						}
					}
				}
				return true;
			}
			catch (Exception ex)
			{
				Platform.Log(LogLevel.Debug, ex, "Unexpected exception encountered while checking for supported gantry tilts");
				return false;
			}
		}
	}
}