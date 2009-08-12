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

using System;
using System.Collections.Generic;
using System.Drawing;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Graphics;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	using DicomImagePlaneDataCache = System.Collections.Generic.Dictionary<string, DicomImagePlane>;

	/// <summary>
	/// An adapter to unify the interface of dicom presentation images
	/// that have valid slice information (e.g. in 3D patient coordinate system).
	/// </summary>
	internal class DicomImagePlane
	{
		#region Private Fields

		private static readonly DicomImagePlaneDataCache _imagePlaneDataCache = new DicomImagePlaneDataCache();
		private static int _referenceCount = 0;

		private IPresentationImage _sourceImage;
		private SpatialTransform _sourceImageTransform;
		private Frame _sourceFrame;

		private Vector3D _normal;
		private Vector3D _positionPatientTopLeft;
		private Vector3D _positionPatientTopRight;
		private Vector3D _positionPatientBottomLeft;
		private Vector3D _positionPatientBottomRight;
		private Vector3D _positionPatientCenterOfImage;
		private Vector3D _positionImagePlaneTopLeft;

		#endregion

		private DicomImagePlane() {}

		public static void InitializeCache()
		{
			++_referenceCount;
		}

		public static void ReleaseCache()
		{
			if (_referenceCount > 0)
				--_referenceCount;

			if (_referenceCount == 0)
				_imagePlaneDataCache.Clear();
		}

		#region Factory Method

		public static DicomImagePlane FromImage(IPresentationImage sourceImage)
		{
			if (sourceImage == null)
				return null;

			Frame frame = GetFrame(sourceImage);
			SpatialTransform transform = GetSpatialTransform(sourceImage);

			if (transform == null || frame == null)
				return null;

			if (String.IsNullOrEmpty(frame.FrameOfReferenceUid) || String.IsNullOrEmpty(frame.ParentImageSop.StudyInstanceUID))
				return null;

			DicomImagePlane plane;
			if (_referenceCount > 0)
				plane = CreateFromCache(frame);
			else
				plane = CreateFromFrame(frame);

			if (plane != null)
			{
				plane._sourceImage = sourceImage;
				plane._sourceImageTransform = transform;
				plane._sourceFrame = frame;
			}

			return plane;
		}

		#endregion

		#region Private Methods

		private static SpatialTransform GetSpatialTransform(IPresentationImage image)
		{
			if (image is ISpatialTransformProvider)
				return ((ISpatialTransformProvider) image).SpatialTransform as SpatialTransform;

			return null;
		}

		private static Frame GetFrame(IPresentationImage image)
		{
			if (image is IImageSopProvider)
				return ((IImageSopProvider) image).Frame;

			return null;
		}

		private static DicomImagePlane CreateFromCache(Frame frame)
		{
			string key = String.Format("{0}:{1}", frame.ParentImageSop.SopInstanceUID, frame.FrameNumber);

			DicomImagePlane cachedData;
			if (_imagePlaneDataCache.ContainsKey(key))
			{
				cachedData = _imagePlaneDataCache[key];
			}
			else
			{
				cachedData = CreateFromFrame(frame);
				if (cachedData != null)
					_imagePlaneDataCache[key] = cachedData;
			}

			if (cachedData != null)
			{
				DicomImagePlane plane = new DicomImagePlane();
				plane.InitializeWithCachedData(cachedData);
				return plane;
			}

			return null;
		}

		private static DicomImagePlane CreateFromFrame(Frame frame)
		{
			int height = frame.Rows - 1;
			int width = frame.Columns - 1;

			DicomImagePlane plane = new DicomImagePlane();
			plane.PositionPatientTopLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, 0));
			plane.PositionPatientTopRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, 0));
			plane.PositionPatientBottomLeft = frame.ImagePlaneHelper.ConvertToPatient(new PointF(0, height));
			plane.PositionPatientBottomRight = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width, height));
			plane.PositionPatientCenterOfImage = frame.ImagePlaneHelper.ConvertToPatient(new PointF(width/2F, height/2F));

			plane.Normal = frame.ImagePlaneHelper.GetNormalVector();

			if (plane.Normal == null || plane.PositionPatientCenterOfImage == null)
				return null;

			// here, we want the position in the coordinate system of the image plane, 
			// without moving the origin (e.g. leave it at the patient origin).
			plane.PositionImagePlaneTopLeft = frame.ImagePlaneHelper.ConvertToImagePlane(plane.PositionPatientTopLeft, Vector3D.Null);

			return plane;
		}

		private void InitializeWithCachedData(DicomImagePlane cachedData)
		{
			Normal = cachedData.Normal;
			PositionPatientTopLeft = cachedData.PositionPatientTopLeft;
			PositionPatientTopRight = cachedData.PositionPatientTopRight;
			PositionPatientBottomLeft = cachedData.PositionPatientBottomLeft;
			PositionPatientBottomRight = cachedData.PositionPatientBottomRight;
			PositionPatientCenterOfImage = cachedData.PositionPatientCenterOfImage;
			PositionImagePlaneTopLeft = cachedData.PositionImagePlaneTopLeft;
		}

		#endregion

		#region Public Properties

		public IPresentationImage SourceImage
		{
			get { return _sourceImage; }
		}

		public SpatialTransform SourceImageTransform
		{
			get { return _sourceImageTransform; }
		}

		public string StudyInstanceUid
		{
			get { return _sourceFrame.ParentImageSop.StudyInstanceUID; }
		}

		public string SeriesInstanceUid
		{
			get { return _sourceFrame.ParentImageSop.SeriesInstanceUID; }
		}

		public string SopInstanceUid
		{
			get { return _sourceFrame.ParentImageSop.SopInstanceUID; }
		}

		public int InstanceNumber
		{
			get { return _sourceFrame.ParentImageSop.InstanceNumber; }
		}

		public int FrameNumber
		{
			get { return _sourceFrame.FrameNumber; }
		}

		public string FrameOfReferenceUid
		{
			get { return _sourceFrame.FrameOfReferenceUid; }
		}

		public float Thickness
		{
			get { return (float) _sourceFrame.SliceThickness; }
		}

		public float Spacing
		{
			get { return (float) _sourceFrame.SpacingBetweenSlices; }
		}

		public Vector3D Normal
		{
			get { return _normal; }
			private set { _normal = value; }
		}

		public Vector3D PositionPatientTopLeft
		{
			get { return _positionPatientTopLeft; }
			private set { _positionPatientTopLeft = value; }
		}

		public Vector3D PositionPatientTopRight
		{
			get { return _positionPatientTopRight; }
			private set { _positionPatientTopRight = value; }
		}

		public Vector3D PositionPatientBottomLeft
		{
			get { return _positionPatientBottomLeft; }
			private set { _positionPatientBottomLeft = value; }
		}

		public Vector3D PositionPatientBottomRight
		{
			get { return _positionPatientBottomRight; }
			private set { _positionPatientBottomRight = value; }
		}

		public Vector3D PositionPatientCenterOfImage
		{
			get { return _positionPatientCenterOfImage; }
			private set { _positionPatientCenterOfImage = value; }
		}

		public Vector3D PositionImagePlaneTopLeft
		{
			get { return _positionImagePlaneTopLeft; }
			private set { _positionImagePlaneTopLeft = value; }
		}

		#endregion

		#region Public Methods

		public Vector3D ConvertToPatient(PointF imagePoint)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToPatient(imagePoint);
		}

		public Vector3D ConvertToImagePlane(Vector3D positionPatient)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToImagePlane(positionPatient);
		}

		public Vector3D ConvertToImagePlane(Vector3D positionPatient, Vector3D originPatient)
		{
			return _sourceFrame.ImagePlaneHelper.ConvertToImagePlane(positionPatient, originPatient);
		}

		public PointF ConvertToImage(PointF positionMillimetres)
		{
			return (PointF) _sourceFrame.ImagePlaneHelper.ConvertToImage(positionMillimetres);
		}

		public bool IsInSameFrameOfReference(DicomImagePlane other)
		{
			Frame otherFrame = other._sourceFrame;

			if (_sourceFrame.ParentImageSop.StudyInstanceUID != otherFrame.ParentImageSop.StudyInstanceUID)
				return false;

			return this._sourceFrame.FrameOfReferenceUid == otherFrame.FrameOfReferenceUid;
		}

		public bool IsParallelTo(DicomImagePlane other, float angleTolerance)
		{
			return Normal.IsParallelTo(other.Normal, angleTolerance);
		}

		public bool IsOrthogonalTo(DicomImagePlane other, float angleTolerance)
		{
			return Normal.IsOrthogonalTo(other.Normal, angleTolerance);
		}

		public float GetAngleBetween(DicomImagePlane other)
		{
			return Normal.GetAngleBetween(other.Normal);
		}

		public bool GetIntersectionPoints(DicomImagePlane other, out Vector3D intersectionPointPatient1, out Vector3D intersectionPointPatient2)
		{
			intersectionPointPatient1 = intersectionPointPatient2 = null;

			Vector3D[,] lineSegmentsImagePlaneBounds = new Vector3D[,]
				{
					// Bounding line segments of this (reference) image plane.
					{PositionPatientTopLeft, PositionPatientTopRight},
					{PositionPatientTopLeft, PositionPatientBottomLeft},
					{PositionPatientBottomRight, PositionPatientTopRight},
					{PositionPatientBottomRight, PositionPatientBottomLeft}
				};

			List<Vector3D> planeIntersectionPoints = new List<Vector3D>();

			for (int i = 0; i < 4; ++i)
			{
				// Intersect the bounding line segments of the reference image with the plane of the target image.
				Vector3D intersectionPoint = Vector3D.GetLinePlaneIntersection(other.Normal, other.PositionPatientCenterOfImage,
				                                                               lineSegmentsImagePlaneBounds[i, 0],
				                                                               lineSegmentsImagePlaneBounds[i, 1], true);
				if (intersectionPoint != null)
					planeIntersectionPoints.Add(intersectionPoint);
			}

			if (planeIntersectionPoints.Count < 2)
				return false;

			intersectionPointPatient1 = planeIntersectionPoints[0];
			intersectionPointPatient2 = CollectionUtils.SelectFirst(planeIntersectionPoints,
			                                                        delegate(Vector3D point) { return !planeIntersectionPoints[0].Equals(point); });

			return intersectionPointPatient1 != null && intersectionPointPatient2 != null;
		}

		#endregion
	}
}