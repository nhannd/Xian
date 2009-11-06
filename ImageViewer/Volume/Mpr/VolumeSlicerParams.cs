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
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//TODO (cr Oct 2009): just properties on Slicer?  Might factor itself out when Slicer creates Slice objects.
	/// <summary>
	/// Defines the parameters for a particular slicing of a <see cref="Volume"/> object (i.e. plane boundaries, orientation, thickness, etc.)
	/// </summary>
	public partial class VolumeSlicerParams : IVolumeSlicerParams
	{
		/// <summary>
		/// Gets the identity slice plane orientation, which is (X=0, Y=0, Z=0).
		/// </summary>
		public static readonly IVolumeSlicerParams Identity = new VolumeSlicerParams().AsReadOnly();

		/// <summary>
		/// Gets an orthogonal slice plane orientation along the X axis in the original frame, which is (X=90, Y=0, Z=0).
		/// </summary>
		public static readonly IVolumeSlicerParams OrthogonalX = new VolumeSlicerParams(90, 0, 0, string.Format(SR.FormatSliceOrthogonalX, 90, 0, 0)).AsReadOnly();

		/// <summary>
		/// Gets an orthogonal slice plane orientation along the Y axis in the original frame, which is (X=90, Y=0, Z=90).
		/// </summary>
		public static readonly IVolumeSlicerParams OrthogonalY = new VolumeSlicerParams(90, 0, 90, string.Format(SR.FormatSliceOrthogonalY, 90, 0, 90)).AsReadOnly();

		private const float _radiansPerDegree = (float) (Math.PI/180);
		private const float _degreesPerRadian = (float) (180/Math.PI);

		private Vector3D _sliceThroughPointPatient;
		private float _sliceExtentXmm;
		private float _sliceExtentYmm;
		private float _sliceSpacing;

		private Matrix _slicingPlaneRotation;
		private float _rotateAboutX;
		private float _rotateAboutY;
		private float _rotateAboutZ;

		private string _description;

		private VolumeSlicerInterpolationMode _interpolationMode = VolumeSlicerInterpolationMode.Linear;

		public VolumeSlicerParams()
		{
			_slicingPlaneRotation = Matrix.GetIdentity(4);
			_rotateAboutX = _rotateAboutY = _rotateAboutZ = 0;
			_description = string.Format(SR.FormatSliceIdentity, 0, 0, 0);
		}

		public VolumeSlicerParams(float rotateAboutX, float rotateAboutY, float rotateAboutZ)
			: this(rotateAboutX, rotateAboutY, rotateAboutZ, string.Format(SR.FormatSliceCustom, rotateAboutX, rotateAboutY, rotateAboutZ)) {}

		public VolumeSlicerParams(float rotateAboutX, float rotateAboutY, float rotateAboutZ, string description)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateAboutX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateAboutY);
			Matrix aboutZ = CalcRotateMatrixAboutZ(rotateAboutZ);

			_slicingPlaneRotation = aboutX*aboutY*aboutZ;
			_rotateAboutX = rotateAboutX;
			_rotateAboutY = rotateAboutY;
			_rotateAboutZ = rotateAboutZ;

			_description = description;
		}

		public VolumeSlicerParams(Matrix sliceRotation, string description)
			: this(sliceRotation)
		{
			_description = description;
		}

		public VolumeSlicerParams(Matrix sliceRotation)
		{
			const string invalidTransformMessage = "sliceRotation must be a 4x4 affine transformation matrix.";

			Platform.CheckForNullReference(sliceRotation, "sliceRotation");
			Platform.CheckTrue(sliceRotation.Columns == 4 && sliceRotation.Rows == 4, invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 0], 0), invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 1], 0), invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 2], 0), invalidTransformMessage);
			Platform.CheckTrue(FloatComparer.AreEqual(sliceRotation[3, 3], 1), invalidTransformMessage);

			_slicingPlaneRotation = sliceRotation;

			double yRadians = (float) Math.Asin(sliceRotation[0, 2]);
			double cosY = Math.Cos(yRadians);
			_rotateAboutX = (float) Math.Atan2(-sliceRotation[1, 2]/cosY, sliceRotation[2, 2]/cosY)*_degreesPerRadian;
			_rotateAboutY = (float) yRadians*_degreesPerRadian;
			_rotateAboutZ = (float) Math.Atan2(-sliceRotation[0, 1]/cosY, sliceRotation[0, 0]/cosY)*_degreesPerRadian;

			_description = string.Format(SR.FormatSliceCustom, _rotateAboutX, _rotateAboutY, _rotateAboutZ);
		}

		public string Description
		{
			get { return _description; }
			set { _description = value; }
		}

		public Matrix SlicingPlaneRotation
		{
			get { return _slicingPlaneRotation; }
			set { _slicingPlaneRotation = value; }
		}

		public Vector3D SliceThroughPointPatient
		{
			get { return _sliceThroughPointPatient; }
			set { _sliceThroughPointPatient = value; }
		}

		public float SliceExtentXMillimeters
		{
			get { return _sliceExtentXmm; }
			set { _sliceExtentXmm = value; }
		}

		public float SliceExtentYMillimeters
		{
			get { return _sliceExtentYmm; }
			set { _sliceExtentYmm = value; }
		}

		public float SliceSpacing
		{
			get { return _sliceSpacing; }
			set { _sliceSpacing = value; }
		}

		public VolumeSlicerInterpolationMode InterpolationMode
		{
			get { return _interpolationMode; }
			set { _interpolationMode = value; }
		}

		bool IVolumeSlicerParams.IsReadOnly
		{
			get { return false; }
		}

		public float RotateAboutX
		{
			get { return _rotateAboutX; }
		}

		public float RotateAboutY
		{
			get { return _rotateAboutY; }
		}

		public float RotateAboutZ
		{
			get { return _rotateAboutZ; }
		}

		public IVolumeSlicerParams AsReadOnly()
		{
			return new ReadOnlyVolumeSlicerParams(this);
		}

		/// <summary>
		/// Allows specification of the slice plane, through point, and extent via two points in patient space
		/// </summary>
		public static VolumeSlicerParams Create(Volume volume, Vector3D sourceOrientationColumnPatient, Vector3D sourceOrientationRowPatient,
		                                               Vector3D startPointPatient, Vector3D endPointPatient)
		{
			Vector3D sourceOrientationNormalPatient = sourceOrientationColumnPatient.Cross(sourceOrientationRowPatient);
			Vector3D normalLinePatient = (endPointPatient - startPointPatient).Normalize();
			Vector3D normalPerpLinePatient = sourceOrientationNormalPatient.Cross(normalLinePatient);

			Vector3D slicePlanePatientX = normalLinePatient;
			Vector3D slicePlanePatientY = sourceOrientationNormalPatient;
			Vector3D slicePlanePatientZ = normalPerpLinePatient;

			Matrix slicePlanePatientOrientation = Math3D.OrientationMatrixFromVectors(slicePlanePatientX, slicePlanePatientY, slicePlanePatientZ);

			Matrix _resliceAxes = volume.RotateToVolumeOrientation(slicePlanePatientOrientation);
			Vector3D lineMiddlePointPatient = new Vector3D(
				(startPointPatient.X + endPointPatient.X)/2,
				(startPointPatient.Y + endPointPatient.Y)/2,
				(startPointPatient.Z + endPointPatient.Z)/2);

			VolumeSlicerParams slicerParams = new VolumeSlicerParams(_resliceAxes);

			slicerParams.SliceThroughPointPatient = new Vector3D(lineMiddlePointPatient);
			slicerParams.SliceExtentXMillimeters = (endPointPatient - startPointPatient).Magnitude;

			return slicerParams;
		}

		#region Static Helpers

		private static Matrix CalcRotateMatrixAboutX(float rotateXdegrees)
		{
			Matrix aboutX;

			if (rotateXdegrees != 0)
			{
				float rotateXradians = rotateXdegrees*_radiansPerDegree;
				float sinX = (float) Math.Sin(rotateXradians);
				float cosX = (float) Math.Cos(rotateXradians);
				aboutX = new Matrix(new float[4,4]
				                    	{
				                    		{1, 0, 0, 0},
				                    		{0, cosX, -sinX, 0},
				                    		{0, sinX, cosX, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutX = Matrix.GetIdentity(4);

			return aboutX;
		}

		private static Matrix CalcRotateMatrixAboutY(float rotateYdegrees)
		{
			Matrix aboutY;

			if (rotateYdegrees != 0)
			{
				float rotateYradians = rotateYdegrees*_radiansPerDegree;
				float sinY = (float) Math.Sin(rotateYradians);
				float cosY = (float) Math.Cos(rotateYradians);
				aboutY = new Matrix(new float[4,4]
				                    	{
				                    		{cosY, 0, sinY, 0},
				                    		{0, 1, 0, 0},
				                    		{-sinY, 0, cosY, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutY = Matrix.GetIdentity(4);

			return aboutY;
		}

		private static Matrix CalcRotateMatrixAboutZ(float rotateZdegrees)
		{
			Matrix aboutZ;

			if (rotateZdegrees != 0)
			{
				float rotateZradians = rotateZdegrees*_radiansPerDegree;
				float sinZ = (float) Math.Sin(rotateZradians);
				float cosZ = (float) Math.Cos(rotateZradians);
				aboutZ = new Matrix(new float[4,4]
				                    	{
				                    		{cosZ, -sinZ, 0, 0},
				                    		{sinZ, cosZ, 0, 0},
				                    		{0, 0, 1, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutZ = Matrix.GetIdentity(4);

			return aboutZ;
		}

		#endregion
	}
}