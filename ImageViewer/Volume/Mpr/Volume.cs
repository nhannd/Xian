#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	//ggerade ToRef: I envision this being the general volume class for all of our 3D needs. It should
	//  probably live in a more general assembly (e.g. ImageViewer.Volume)
	/// <summary>
	/// The Volume class encapsulates 3 dimensional pixel data. Use the VolumeBuilder class to create a Volume.
	/// </summary>
	public class Volume : IDisposable
	{
		#region Private fields

		// Note: Constructors ensure that only one of these arrays is set.
		//	This defines whether the data is signed or unsigned.
		private readonly short[] _volShortArray;
		private readonly ushort[] _volUnsignedShortArray;
		private readonly Size3D _arrayDimensions;
		private readonly Vector3D _spacing;
		private readonly Vector3D _originPatient;
		private readonly Matrix _orientationPatientMatrix;
		private readonly DicomMessageBase _modelDicom;
		private readonly int _padValue;
		// Decided to keep the volume origin at 0,0,0 and translate to patient coordinates
		//	when needed. This mimics the typical image to patient coordintate transform.
		private readonly Vector3D _origin = new Vector3D(0, 0, 0);

		// As pertinent volume fields are currently readonly, we only have to create the
		//	VTK volume wrapper once for each volume.
		private vtkImageData _cachedVtkVolume;

		// This handle is used to pin the volume array. Whenever VTK operates on the volume,
		//	the volume array must be pinned.
		private GCHandle _volArrayPinnedHandle;

		private bool _disposed;

		#endregion

		#region Initialization

		internal Volume(short[] volShortArray, Size3D arrayDimensions, Vector3D spacing, Vector3D originPatient,
		                Matrix orientationPateintMatrix, DicomMessageBase modelDicom, int padValue)
			: this(arrayDimensions, spacing, originPatient, orientationPateintMatrix, modelDicom, padValue)
		{
			_volShortArray = volShortArray;
			//ggerade ToOpt: I think it would be better to pin these only while necessary. Consider refactoring
			//	such that getting vtkImageData pins, then a call to release
			_volArrayPinnedHandle = GCHandle.Alloc(_volShortArray, GCHandleType.Pinned);
		}

		internal Volume(ushort[] volUnsignedShortArray, Size3D arrayDimensions, Vector3D spacing, Vector3D originPatient,
		                Matrix orientationPatientMatrix, DicomMessageBase modelDicom, int padValue)
			: this(arrayDimensions, spacing, originPatient, orientationPatientMatrix, modelDicom, padValue)
		{
			_volUnsignedShortArray = volUnsignedShortArray;
			_volArrayPinnedHandle = GCHandle.Alloc(_volUnsignedShortArray, GCHandleType.Pinned);
		}

		private Volume(Size3D arrayDimensions, Vector3D spacing, Vector3D originPatient, Matrix orientationPatientMatrix,
		               DicomMessageBase modelDicom, int padValue)
		{
			_arrayDimensions = arrayDimensions;
			_spacing = spacing;
			_originPatient = originPatient;
			_orientationPatientMatrix = orientationPatientMatrix;
			_modelDicom = modelDicom;
			_padValue = padValue;
		}

		#endregion

		#region Public properties

		// Effective volume dimensions (VTK output will take spacing into account for us)
		public Vector3D Dimensions
		{
			get
			{
				return new Vector3D(ArrayDimensions.Width * Spacing.X, ArrayDimensions.Height * Spacing.Y,
				                    ArrayDimensions.Depth * Spacing.Z);
			}
		}

		public Vector3D Spacing
		{
			get { return _spacing; }
		}

		public Vector3D OriginPatient
		{
			get { return _originPatient; }
		}

		public Matrix OrientationPatientMatrix
		{
			get { return _orientationPatientMatrix; }
		}

		public bool IsDataUnsigned()
		{
			return _volUnsignedShortArray != null;
		}

		public int SizeInVoxels
		{
			get { return ArrayDimensions.Size; }
		}

		public float MinXCoord
		{
			get { return Origin.X; }
		}

		public float MaxXCoord
		{
			get { return (Origin.X + Spacing.X * ArrayDimensions.Width); }
		}

		public float MinYCoord
		{
			get { return Origin.Y; }
		}

		public float MaxYCoord
		{
			get { return (Origin.Y + Spacing.Y * ArrayDimensions.Height); }
		}

		public float MinZCoord
		{
			get { return Origin.Z; }
		}

		public float MaxZCoord
		{
			get { return (Origin.Z + Spacing.Z * ArrayDimensions.Depth); }
		}

		public float EffectiveSpacing
		{
			get { return Math.Min(Math.Min(Spacing.X, Spacing.Y), Spacing.Z); }
		}

		public float MaxSpacing
		{
			get { return Math.Max(Math.Max(Spacing.X, Spacing.Y), Spacing.Z); }
		}

		public Vector3D Origin
		{
			get { return _origin; }
		}

		public int PadValue
		{
			get { return _padValue; }
		}

		#endregion

		#region Public methods

		public Vector3D CenterPoint
		{
			get
			{
				// Volume center point
				Vector3D center = new Vector3D(Origin.X + Spacing.X * 0.5f * ArrayDimensions.Width,
				                               Origin.Y + Spacing.Y * 0.5f * ArrayDimensions.Height,
				                               Origin.Z + Spacing.Z * 0.5f * ArrayDimensions.Depth);
				return center;
			}
		}

		public Vector3D CenterPointPatient
		{
			get { return ConvertToPatient(CenterPoint); }
		}

		public Vector3D ConvertToPatient(Vector3D volumePosition)
		{
			// Set orientation transform
			Matrix volumePatientTransform = new Matrix(OrientationPatientMatrix);
			// Set origin translation
			volumePatientTransform.SetRow(3, OriginPatient.X, OriginPatient.Y, OriginPatient.Z, 1);

			// Transform volume position to patient position
			Matrix imagePositionMatrix = new Matrix(1, 4);
			imagePositionMatrix.SetRow(0, volumePosition.X, volumePosition.Y, volumePosition.Z, 1F);
			Matrix patientPositionMatrix = imagePositionMatrix * volumePatientTransform;

			Vector3D patientPosition = new Vector3D(patientPositionMatrix[0, 0], patientPositionMatrix[0, 1],
			                                        patientPositionMatrix[0, 2]);
			return patientPosition;
		}

		public Vector3D ConvertToVolume(Vector3D patientPosition)
		{
			// Set orientation transform
			Matrix patientVolumeTransform = new Matrix(OrientationPatientMatrix.Transpose());
			// Set origin translation
			patientVolumeTransform.SetRow(3, -OriginPatient.X, -OriginPatient.Y, -OriginPatient.Z, 1);

			// Transform patient position to volume position
			Matrix patientPositionMatrix = new Matrix(1, 4);
			patientPositionMatrix.SetRow(0, patientPosition.X, patientPosition.Y, patientPosition.Z, 1F);
			Matrix imagePositionMatrix = patientPositionMatrix * patientVolumeTransform;

			Vector3D imagePosition = new Vector3D(imagePositionMatrix[0, 0], imagePositionMatrix[0, 1],
			                                      imagePositionMatrix[0, 2]);
			return imagePosition;
		}

		public Matrix RotateToPatientOrientation(Matrix orientationVolume)
		{
			Matrix orientationPatient = orientationVolume * OrientationPatientMatrix;
			return orientationPatient;
		}

		public Matrix RotateToVolumeOrientation(Matrix orientationPatient)
		{
			Matrix orientationVolume = orientationPatient * OrientationPatientMatrix.Transpose();
			return orientationVolume;
		}

		public Vector3D RotateToPatientOrientation(Vector3D volumeVec)
		{
			Matrix volumePos = new Matrix(4, 1);
			volumePos.SetColumn(0, volumeVec.X, volumeVec.Y, volumeVec.Z, 1F);
			Matrix patientPos = OrientationPatientMatrix * volumePos;
			return new Vector3D(patientPos[0, 0], patientPos[1, 0], patientPos[2, 0]);
		}

		public float LongAxisMagnitude
		{
			get { return Math.Max(Math.Max(Dimensions.X, Dimensions.Y), Dimensions.Z); }
		}

		public float ShortAxisMagnitude
		{
			get { return Math.Min(Math.Min(Dimensions.X, Dimensions.Y), Dimensions.Z); }
		}

		public float DiagonalMagnitude
		{
			get
			{
				return (float) Math.Sqrt(Dimensions.X * Dimensions.X +
				                         Dimensions.Y * Dimensions.Y +
				                         Dimensions.Z * Dimensions.Z);
			}
		}

		public float LargestOrthogonalPlaneDiagonal
		{
			get
			{
				//ggerade ToOpt: This doesn't seem like the most intelligent way to determine this (it was late :)
				if (Dimensions.Y > Dimensions.X)
					if (Dimensions.Z > Dimensions.X)
						return (float) Math.Sqrt(Dimensions.Y * Dimensions.Y + Dimensions.Z * Dimensions.Z);
					else
						return (float) Math.Sqrt(Dimensions.Y * Dimensions.Y + Dimensions.X * Dimensions.X);
				else if (Dimensions.Z > Dimensions.Y)
					return (float) Math.Sqrt(Dimensions.X * Dimensions.X + Dimensions.Z * Dimensions.Z);
				else
					return (float) Math.Sqrt(Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y);
			}
		}

		#endregion

		#region Implementation

		internal DicomMessageBase _ModelDicom
		{
			get { return _modelDicom; }
		}

		internal vtkImageData _VtkVolume
		{
			get
			{
				if (_cachedVtkVolume == null)
					_cachedVtkVolume = CreateVtkVolume();
				return _cachedVtkVolume;
			}
		}

		internal int LargestOutputImageDimension
		{
			get
			{
				// This doesn't give is much extra room, so I decided to use the diagonal along long and short dimensions
				//return (int)(LongAxisMagnitude / EffectiveSpacing + 0.5f);
				float longOutputDimension = LongAxisMagnitude / EffectiveSpacing;
				float shortOutputDimenstion = ShortAxisMagnitude / EffectiveSpacing;
				return (int) Math.Sqrt(longOutputDimension * longOutputDimension + shortOutputDimenstion * shortOutputDimenstion);
			}
		}

		// Decided to keep private for now, shouldn't be interesting to the outside world, and helps 
		//	avoid confusion with dimensions that take spacing into account (which is useful to the outside world)
		private Size3D ArrayDimensions
		{
			get { return _arrayDimensions; }
		}

		#endregion

		#region VTK stuff

		private vtkImageData CreateVtkVolume()
		{
			vtkImageData vtkVolume = new vtkImageData();

			//ggerade ToRes: This attempt to capture vtkError OutputWindow failed...
			//vtkVolume.AddObserver(123, VtkVolumeCallback);

			vtkVolume.SetDimensions(ArrayDimensions.Width, ArrayDimensions.Height, ArrayDimensions.Depth);
			vtkVolume.SetOrigin(Origin.X, Origin.Y, Origin.Z);
			vtkVolume.SetSpacing(Spacing.X, Spacing.Y, Spacing.Z);

			if (IsDataUnsigned())
			{
				vtkVolume.SetScalarTypeToUnsignedShort();
				vtkVolume.GetPointData().SetScalars(CreateVtkUnsignedShortArrayWrapper(_volUnsignedShortArray));
			}
			else
			{
				vtkVolume.SetScalarTypeToShort();
				vtkVolume.GetPointData().SetScalars(CreateVtkShortArrayWrapper(_volShortArray));
			}

			// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
			vtkVolume.UpdateInformation();

			return vtkVolume;
		}

		// This didn't get called when errors occur, need to investigate handling of VTK errors further
		//private static void VtkVolumeCallback(vtkObject vtkObj, uint eid, object obj, IntPtr nativeSomethingOrOther)
		//{
		//    Debug.WriteLine(eid);
		//}

		private static vtkShortArray CreateVtkShortArrayWrapper(short[] shortArray)
		{
			vtkShortArray vtkShortArray = new vtkShortArray();
			vtkShortArray.SetArray(shortArray, shortArray.Length, 1);
			return vtkShortArray;
		}

		private static vtkUnsignedShortArray CreateVtkUnsignedShortArrayWrapper(ushort[] ushortArray)
		{
			vtkUnsignedShortArray vtkUnsignedShortArray = new vtkUnsignedShortArray();
			vtkUnsignedShortArray.SetArray(ushortArray, ushortArray.Length, 1);
			return vtkUnsignedShortArray;
		}

		#endregion

		#region Disposal

		public void Dispose()
		{
			try
			{
				Dispose(true);
				GC.SuppressFinalize(this);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}
		}

		protected void Dispose(bool disposing)
		{
			if (disposing && !_disposed)
			{
				_disposed = true;
				//can only call Free once or it throws
				_volArrayPinnedHandle.Free();
				if (_cachedVtkVolume != null)
				{
					_cachedVtkVolume.GetPointData().Dispose();
					_cachedVtkVolume.Dispose();
				}
			}
		}

		#endregion
	}
}