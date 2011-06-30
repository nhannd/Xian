#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591,0419,1574,1587

using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	public class DataSetOrientation
	{
		private readonly Vector3D _imageXPatient;
		private readonly Vector3D _imageYPatient;
		private readonly Vector3D _stackZPatient;
		private int _sliceNumber = 0;

		/// <summary>
		/// Constructs a <see cref="DataSetOrientation"/> using the patient space as a basis for the stack coordinate space.
		/// </summary>
		public DataSetOrientation() : this(Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit) {}

		/// <summary>
		/// Constructs a <see cref="DataSetOrientation"/> using the given vectors as a basis for the stack coordinate space (in patient space).
		/// </summary>
		/// <param name="imageXPatient">The vector from an image's top left to top right corner, in patient coordinates.</param>
		/// <param name="imageYPatient">The vector from an image's top left to bottom left corner, in patient coordinates.</param>
		/// <param name="stackZPatient">The vector from the first image's top left to the last image's top left corner, in patient coordinates.</param>
		public DataSetOrientation(Vector3D imageXPatient, Vector3D imageYPatient, Vector3D stackZPatient)
		{
			_imageXPatient = imageXPatient.Normalize();
			_imageYPatient = imageYPatient.Normalize();
			_stackZPatient = stackZPatient.Normalize();
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a standard axial stack.
		/// </summary>
		public static DataSetOrientation CreateAxial(bool reverse)
		{
			float multiplier = reverse ? -1f : 1f;
			return new DataSetOrientation(Vector3D.xUnit, Vector3D.yUnit, Vector3D.zUnit*multiplier);
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a standard coronal stack.
		/// </summary>
		public static DataSetOrientation CreateCoronal(bool reverse)
		{
			float multiplier = reverse ? -1f : 1f;
			return new DataSetOrientation(Vector3D.xUnit, -Vector3D.zUnit, Vector3D.yUnit*multiplier);
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a standard sagittal stack.
		/// </summary>
		public static DataSetOrientation CreateSagittal(bool reverse)
		{
			float multiplier = reverse ? -1f : 1f;
			return new DataSetOrientation(-Vector3D.zUnit, Vector3D.yUnit, Vector3D.xUnit*multiplier);
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a gantry tilted stack rotated about the patient's X axis.
		/// </summary>
		public static DataSetOrientation CreateGantryTiltedAboutX(double tiltDegrees)
		{
			double tiltRadians = tiltDegrees*Math.PI/180;
			Vector3D yVector = new Vector3D(0, (float) Math.Cos(tiltRadians), (float) -Math.Sin(tiltRadians));
			return new DataSetOrientation(Vector3D.xUnit, yVector, Vector3D.zUnit);
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a gantry tilted stack rotated about the patient's Y axis.
		/// </summary>
		public static DataSetOrientation CreateGantryTiltedAboutY(double tiltDegrees)
		{
			double tiltRadians = tiltDegrees*Math.PI/180;
			Vector3D xVector = new Vector3D((float) Math.Cos(tiltRadians), 0, (float) -Math.Sin(tiltRadians));
			return new DataSetOrientation(xVector, Vector3D.yUnit, Vector3D.zUnit);
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a couch tilted stack rotated about the patient's X axis.
		/// </summary>
		public static DataSetOrientation CreateCouchTiltedAboutX(double tiltDegrees)
		{
			double tiltRadians = tiltDegrees*Math.PI/180;
			Vector3D yVector = new Vector3D(0, (float) Math.Cos(tiltRadians), (float) -Math.Sin(tiltRadians));
			Vector3D zVector = new Vector3D(0, (float) Math.Sin(tiltRadians), (float) Math.Cos(tiltRadians));
			return new DataSetOrientation(Vector3D.xUnit, yVector, zVector);
		}

		/// <summary>
		/// Creates a <see cref="DataSetOrientation"/> for a couch tilted stack rotated about the patient's Y axis.
		/// </summary>
		public static DataSetOrientation CreateCouchTiltedAboutY(double tiltDegrees)
		{
			double tiltRadians = tiltDegrees*Math.PI/180;
			Vector3D xVector = new Vector3D((float) Math.Cos(tiltRadians), 0, (float) -Math.Sin(tiltRadians));
			Vector3D zVector = new Vector3D((float) Math.Sin(tiltRadians), 0, (float) Math.Cos(tiltRadians));
			return new DataSetOrientation(xVector, Vector3D.yUnit, zVector);
		}

		public void Initialize(ISopDataSource sopDataSource)
		{
			sopDataSource[DicomTags.ImageOrientationPatient].SetStringValue(this.ImageOrientationPatient);
			sopDataSource[DicomTags.ImagePositionPatient].SetStringValue(this.NextImagePositionPatient());
		}

		public string ImageOrientationPatient
		{
			get
			{
				return string.Format(@"{0:f6}\{1:f6}\{2:f6}\{3:f6}\{4:f6}\{5:f6}",
				                     Math.Cos(Math.Abs(_imageXPatient.GetSubtendedAngle(Vector3D.xUnit))),
                                     Math.Cos(Math.Abs(_imageXPatient.GetSubtendedAngle(Vector3D.yUnit))),
                                     Math.Cos(Math.Abs(_imageXPatient.GetSubtendedAngle(Vector3D.zUnit))),
                                     Math.Cos(Math.Abs(_imageYPatient.GetSubtendedAngle(Vector3D.xUnit))),
                                     Math.Cos(Math.Abs(_imageYPatient.GetSubtendedAngle(Vector3D.yUnit))),
                                     Math.Cos(Math.Abs(_imageYPatient.GetSubtendedAngle(Vector3D.zUnit)))
					);
			}
		}

		public string ImagePositionPatient
		{
			get
			{
				Vector3D imagePosition = _sliceNumber*_stackZPatient;
				return string.Format(@"{0:f6}\{1:f6}\{2:f6}", imagePosition.X, imagePosition.Y, imagePosition.Z);
			}
		}

		public string NextImagePositionPatient()
		{
			try { return this.ImagePositionPatient; } finally {
				_sliceNumber++;
			}
		}

		public void ResetImagePositionPatient()
		{
			_sliceNumber = 0;
		}
	}
}

#endif