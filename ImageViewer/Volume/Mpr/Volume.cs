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
	internal class Volume : IDisposable
	{
		#region Private fields

		// Note: Constructors ensure that only one of these arrays is set.
		//	This defines whether the data is signed or unsigned.
		private readonly short[] _volShortArray;
		private readonly ushort[] _volUnsignedShortArray;
		//ggerade ToRef: Use a 3-tuple of ints instead of floats... (Size3D?)
		private readonly Vector3D _arrayDimensions;
		private readonly Vector3D _spacing;
		private readonly Vector3D _originPatient;
		private readonly Matrix _orientationPatientMatrix;
		private readonly DicomMessageBase _modelDicom;
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

		internal Volume(short[] volShortArray, Vector3D arrayDimensions, Vector3D spacing, Vector3D originPatient,
		                Matrix orientationPateintMatrix, DicomMessageBase modelDicom)
			: this(arrayDimensions, spacing, originPatient, orientationPateintMatrix, modelDicom)
		{
			_volShortArray = volShortArray;
			//ggerade ToOpt: I think it would be better to pin these only while necessary. Consider refactoring
			//	such that getting vtkImageData pins, then a call to release
			_volArrayPinnedHandle = GCHandle.Alloc(_volShortArray, GCHandleType.Pinned);
		}

		internal Volume(ushort[] volUnsignedShortArray, Vector3D arrayDimensions, Vector3D spacing, Vector3D originPatient,
		                Matrix orientationPatientMatrix, DicomMessageBase modelDicom)
			: this(arrayDimensions, spacing, originPatient, orientationPatientMatrix, modelDicom)
		{
			_volUnsignedShortArray = volUnsignedShortArray;
			_volArrayPinnedHandle = GCHandle.Alloc(_volUnsignedShortArray, GCHandleType.Pinned);
		}

		private Volume(Vector3D dimensions, Vector3D arrayDimensions, Vector3D originPatient, Matrix orientationPatientMatrix,
		               DicomMessageBase modelDicom)
		{
			_arrayDimensions = dimensions;
			_spacing = arrayDimensions;
			_originPatient = originPatient;
			_orientationPatientMatrix = orientationPatientMatrix;
			_modelDicom = modelDicom;
		}

		#endregion

		#region Public properties

		// Effective volume dimensions (VTK output will take spacing into account for us)
		public Vector3D Dimensions
		{
			get { return new Vector3D(ArrayDimensions.X * Spacing.X, ArrayDimensions.Y * Spacing.Y, ArrayDimensions.Z * Spacing.Z); }
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

		public DicomMessageBase _ModelDicom
		{
			get { return _modelDicom; }
		}

		public bool IsDataUnsigned()
		{
			return _volUnsignedShortArray != null;
		}

		public vtkImageData _VtkVolume
		{
			get
			{
				if (_cachedVtkVolume == null)
					_cachedVtkVolume = CreateVtkVolume();
				return _cachedVtkVolume;
			}
		}

		public int SizeInVoxels
		{
			get { return (int) (ArrayDimensions.X * ArrayDimensions.Y * ArrayDimensions.Z); }
		}

		public float MinXCoord
		{
			get { return Origin.X; }
		}

		public float MaxXCoord
		{
			get { return (Origin.X + Spacing.X * ArrayDimensions.X); }
		}

		public float MinYCoord
		{
			get { return Origin.Y; }
		}

		public float MaxYCoord
		{
			get { return (Origin.Y + Spacing.Y * ArrayDimensions.Y); }
		}

		public float MinZCoord
		{
			get { return Origin.Z; }
		}

		public float MaxZCoord
		{
			get { return (Origin.Z + Spacing.Z * ArrayDimensions.Z); }
		}

		public float EffectiveSpacing
		{
			get { return Math.Min(Math.Min(Spacing.X, Spacing.Y), Spacing.Z); }
		}

		public Vector3D Origin
		{
			get { return _origin; }
		}

		#endregion

		#region Public methods

		public Vector3D CenterPoint
		{
			get
			{
				// Volume center point
				Vector3D center = new Vector3D(Origin.X + Spacing.X * 0.5f * ArrayDimensions.X,
				                               Origin.Y + Spacing.Y * 0.5f * ArrayDimensions.Y,
				                               Origin.Z + Spacing.Z * 0.5f * ArrayDimensions.Z);
				return center;
			}
		}

		public Vector3D ConvertToPatient(Vector3D volumePosition)
		{
			// Set orientation transform
			Matrix volToPat = new Matrix(OrientationPatientMatrix);
			// Set origin translation
			volToPat.SetRow(3, OriginPatient.X, OriginPatient.Y, OriginPatient.Z, 1);

			// Transform image position to patient position
			Matrix imagePositionMatrix = new Matrix(1, 4);
			imagePositionMatrix.SetRow(0, volumePosition.X, volumePosition.Y, volumePosition.Z, 1F);
			Matrix patientPositionMatrix = imagePositionMatrix * volToPat;

			Vector3D patientPosition = new Vector3D(patientPositionMatrix[0, 0], patientPositionMatrix[0, 1],
			                                        patientPositionMatrix[0, 2]);
			return patientPosition;
		}

		public Matrix RotateToPatientOrientation(Matrix orientationImage)
		{
			Matrix orientationPatient = orientationImage * OrientationPatientMatrix;
			return orientationPatient;
		}

		public float LongAxisMagnitude
		{
			get
			{
				return Math.Max(Math.Max(Dimensions.X, Dimensions.Y), Dimensions.Z);
			}
		}

		public float ShortAxisMagnitude
		{
			get
			{
				return Math.Min(Math.Min(Dimensions.X, Dimensions.Y), Dimensions.Z);
			}
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
				if (Dimensions.Y > Dimensions.X)
					if (Dimensions.Z > Dimensions.X)
						return (float) Math.Sqrt(Dimensions.Y * Dimensions.Y + Dimensions.Z * Dimensions.Z);
					else
						return (float)Math.Sqrt(Dimensions.Y * Dimensions.Y + Dimensions.X * Dimensions.X);
				else
					if (Dimensions.Z > Dimensions.Y)
						return (float)Math.Sqrt(Dimensions.X * Dimensions.X + Dimensions.Z * Dimensions.Z);
					else
						return (float)Math.Sqrt(Dimensions.X * Dimensions.X + Dimensions.Y * Dimensions.Y);
			}
		}

		#endregion

		#region Implementation

		internal int LargestOutputImageDimension
		{
			get
			{
				// This doesn't give is much extra room, so I decided to use the diagonal along long and short dimensions
				//return (int)(LongAxisMagnitude / EffectiveSpacing + 0.5f);
				float longOutputDimension = LongAxisMagnitude / EffectiveSpacing;
				float shortOutputDimenstion = ShortAxisMagnitude / EffectiveSpacing;
				return (int)Math.Sqrt(longOutputDimension * longOutputDimension + shortOutputDimenstion * shortOutputDimenstion);
			}
		}

		// Decided to keep private for now, shouldn't be interesting to the outside world, and helps 
		//	avoid confusion with dimensions that take spacing into account (which is useful to the outside world)
		private Vector3D ArrayDimensions
		{
			get { return _arrayDimensions; }
		}

		#endregion

		#region VTK stuff

		private vtkImageData CreateVtkVolume()
		{
			vtkImageData vtkVolume = new vtkImageData();

			vtkVolume.SetDimensions((int) ArrayDimensions.X, (int) ArrayDimensions.Y, (int) ArrayDimensions.Z);
			vtkVolume.SetOrigin(0, 0, 0);
			vtkVolume.SetSpacing(Spacing.X, Spacing.Y, Spacing.Z);

			if (IsDataUnsigned())
			{
				vtkVolume.SetScalarTypeToUnsignedShort();
				vtkVolume.AllocateScalars();

				vtkVolume.GetPointData().SetScalars(CreateVtkUnsignedShortArrayWrapper(_volUnsignedShortArray));
			}
			else
			{
				vtkVolume.SetScalarTypeToShort();
				vtkVolume.AllocateScalars();

				vtkVolume.GetPointData().SetScalars(CreateVtkShortArrayWrapper(_volShortArray));
			}

			// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
			vtkVolume.UpdateInformation();

			return vtkVolume;
		}

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
					_cachedVtkVolume.Dispose();
			}
		}

		#endregion
	}
}