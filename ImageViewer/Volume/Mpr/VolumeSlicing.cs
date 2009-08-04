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
	public enum InterpolationModes
	{
		NearestNeighbor,
		Linear,
		Cubic
	}

	/// <summary>
	/// Defines the parameters for a particular slicing of a <see cref="Volume"/> (i.e. plane boundaries, orientation, thickness, etc.)
	/// </summary>
	public class VolumeSlicing
	{
		private Matrix _resliceAxes;

		private Vector3D _sliceThroughPointPatient;
		private float _sliceExtentXmm;
		private float _sliceExtentYmm;
		private float _sliceSpacing;

		private int _rotateAboutX;
		private int _rotateAboutY;
		private int _rotateAboutZ;

		private InterpolationModes _interpolationMode = InterpolationModes.Linear;

		public VolumeSlicing() {}

		private VolumeSlicing(Matrix axes)
		{
			Platform.CheckForNullReference(axes, "axes");
			_resliceAxes = axes;
		}

		public Matrix ResliceAxes
		{
			get { return _resliceAxes; }
		}

		public Vector3D SliceThroughPointPatient
		{
			get { return _sliceThroughPointPatient; }
		}

		public float SliceExtentXMillimeters
		{
			get { return _sliceExtentXmm; }
		}

		public float SliceExtentYMillimeters
		{
			get { return _sliceExtentYmm; }
		}

		/// <summary>
		/// Allows specification of the slice plane, through point, and extent via two points in patient space
		/// </summary>
		/// <param name="sourceOrientationColumnPatient"></param>
		/// <param name="sourceOrientationRowPatient"></param>
		/// <param name="startPointPatient"></param>
		/// <param name="endPointPatient"></param>
		public static VolumeSlicing CreateSlicing(Volume volume, Vector3D sourceOrientationColumnPatient, Vector3D sourceOrientationRowPatient,
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

			VolumeSlicing slicing = new VolumeSlicing(_resliceAxes);

			slicing._sliceThroughPointPatient = new Vector3D(lineMiddlePointPatient);
			slicing._sliceExtentXmm = (endPointPatient - startPointPatient).Magnitude;

			return slicing;
		}

		public float SliceSpacing
		{
			get { return _sliceSpacing; }
			set { _sliceSpacing = value; }
		}

		public InterpolationModes InterpolationMode
		{
			get { return _interpolationMode; }
			set { _interpolationMode = value; }
		}

		public int RotateAboutX
		{
			get { return _rotateAboutX; }
		}

		public int RotateAboutY
		{
			get { return _rotateAboutY; }
		}

		public int RotateAboutZ
		{
			get { return _rotateAboutZ; }
		}

		#region Static Helpers

		public static VolumeSlicing CreateIdentitySlicing()
		{
			return new VolumeSlicing(new Matrix(new float[4,4]
			                                    	{
			                                    		{1, 0, 0, 0},
			                                    		{0, 1, 0, 0},
			                                    		{0, 0, 1, 0},
			                                    		{0, 0, 0, 1}
			                                    	}));
		}

		public static VolumeSlicing CreateOrthogonalYSlicing()
		{
			return new VolumeSlicing(new Matrix(new float[4,4]
			                                    	{
			                                    		{0, 1, 0, 0},
			                                    		{0, 0, -1, 0},
			                                    		{1, 0, 0, 0},
			                                    		{0, 0, 0, 1}
			                                    	}));
		}

		public static VolumeSlicing CreateOrthogonalXSlicing()
		{
			return new VolumeSlicing(new Matrix(new float[4,4]
			                                    	{
			                                    		{1, 0, 0, 0},
			                                    		{0, 0, -1, 0},
			                                    		{0, 1, 0, 0},
			                                    		{0, 0, 0, 1}
			                                    	}));
		}

		public static VolumeSlicing CreateSlicing(int rotateX, int rotateY, int rotateZ)
		{
			Matrix aboutX = CalcRotateMatrixAboutX(rotateX);
			Matrix aboutY = CalcRotateMatrixAboutY(rotateY);
			Matrix aboutZ = CalcRotateMatrixAboutZ(rotateZ);

			VolumeSlicing slicing = new VolumeSlicing(aboutX*aboutY*aboutZ);
			slicing._rotateAboutX = rotateX;
			slicing._rotateAboutY = rotateY;
			slicing._rotateAboutZ = rotateZ;
			return slicing;
		}

		private static readonly Matrix _identityMatrix = new Matrix(new float[4,4]
		                                                            	{
		                                                            		{1, 0, 0, 0},
		                                                            		{0, 1, 0, 0},
		                                                            		{0, 0, 1, 0},
		                                                            		{0, 0, 0, 1}
		                                                            	});

		private static Matrix CalcRotateMatrixAboutX(int rotateXdegrees)
		{
			Matrix aboutX;

			if (rotateXdegrees != 0)
			{
				float sinX = (float) Math.Sin(rotateXdegrees*Math.PI/180);
				float cosX = (float) Math.Cos(rotateXdegrees*Math.PI/180);
				aboutX = new Matrix(new float[4,4]
				                    	{
				                    		{1, 0, 0, 0},
				                    		{0, cosX, -sinX, 0},
				                    		{0, sinX, cosX, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutX = _identityMatrix;

			return aboutX;
		}

		private static Matrix CalcRotateMatrixAboutY(int rotateYdegrees)
		{
			Matrix aboutY;

			if (rotateYdegrees != 0)
			{
				float sinY = (float) Math.Sin(rotateYdegrees*Math.PI/180);
				float cosY = (float) Math.Cos(rotateYdegrees*Math.PI/180);
				aboutY = new Matrix(new float[4,4]
				                    	{
				                    		{cosY, 0, sinY, 0},
				                    		{0, 1, 0, 0},
				                    		{-sinY, 0, cosY, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutY = _identityMatrix;

			return aboutY;
		}

		private static Matrix CalcRotateMatrixAboutZ(int rotateZdegrees)
		{
			Matrix aboutZ;

			if (rotateZdegrees != 0)
			{
				float sinZ = (float) Math.Sin(rotateZdegrees*Math.PI/180);
				float cosZ = (float) Math.Cos(rotateZdegrees*Math.PI/180);
				aboutZ = new Matrix(new float[4,4]
				                    	{
				                    		{cosZ, -sinZ, 0, 0},
				                    		{sinZ, cosZ, 0, 0},
				                    		{0, 0, 1, 0},
				                    		{0, 0, 0, 1}
				                    	});
			}
			else
				aboutZ = _identityMatrix;

			return aboutZ;
		}

		#endregion
	}
}