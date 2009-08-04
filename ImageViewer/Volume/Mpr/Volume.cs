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
using System.Runtime.InteropServices;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.ImageViewer.Volume.Mpr.Utilities;
using vtk;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	/// <summary>
	/// The Volume class encapsulates 3 dimensional voxel data and currently supports short and unsigned short types.
	/// You will typically use a <see cref="VolumeBuilder"/> object to create a Volume.
	/// A volume has two coordinate spaces of interest: volume and patient. Naming convention is to use Patient
	/// suffix if in patient space, Volume suffix in volume space, if not specified then in volume space.
	/// 
	/// The spaces currently only differ (potentially) by origin and orientation. The volume origin is 
	/// fixed to 0,0,0 and the patient origin is derived from the first image's DICOM image position (patient).
	/// The volume orientation is consistently defined by the input images and is irrespective of the actual
	/// patient orientation (i.e. axial, sagittal, coronal captured images are all normalized in volume space).
	/// The patient orientation is derived from the DICOM image orientation.
	///  </summary>
	/// TODO JY
	public partial class Volume : IDisposable
	{
		#region Private fields

		// The volume arrays that contain the voxel values. Constructors ensure that only one of these 
		//	arrays is set, which defines whether the data for this volume is signed or unsigned.
		private short[] _volShortArray;
		private ushort[] _volUnsignedShortArray;

		// Size of each of the volume array dimensions
		private readonly Size3D _arrayDimensions;

		// The spacing between pixels (X,Y) and slices (Z) as defined by the DICOM images for this volume
		private readonly Vector3D _spacing;
		// The DICOM image position (patient) of the first slice in the volume, used to convert between Volume and Patient spaces
		private readonly Vector3D _originPatient;
		// The DICOM image orientation (patient) of all slices, used to convert between Volume and Patient spaces
		private readonly Matrix _orientationPatientMatrix;
		// Used as pixel value for any data not derived from voxel values (e.g. when slice extends beyond volume)
		private readonly int _padValue;
		// Decided to keep the volume origin at 0,0,0 and translate to patient coordinates
		//	when needed. This makes dealing with non axial datasets easier. It also mimics 
		//	the typical image to patient coordintate transform.
		private readonly Vector3D _origin = new Vector3D(0, 0, 0);

		// As pertinent volume fields are currently readonly, we only have to create the
		//	VTK volume wrapper once for each volume.
		private vtkImageData _cachedVtkVolume;

		// This handle is used to pin the volume array. Whenever VTK operates on the volume,
		//	the volume array must be pinned.
		private GCHandle? _volArrayPinnedHandle;

		private bool _disposed;

		private readonly string _description;
		private readonly VolumeSopDataSourcePrototype _modelDicom;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of signed 16-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using one of the static helpers such as <see cref="CreateVolume(IDisplaySet)"/> to construct and automatically fill a <see cref="Volume"/>.
		/// </remarks>
		public Volume(short[] data, Size3D dimensions, Vector3D spacing, Vector3D originPatient,
		              Matrix orientationPatient, IDicomAttributeProvider dicomAttributeModel, int paddingValue)
			: this(data, null, dimensions, spacing, originPatient, orientationPatient,
			       VolumeSopDataSourcePrototype.Create(dicomAttributeModel), paddingValue) {}

		/// <summary>
		/// Constructs a <see cref="Volume"/> using a volume data array of unsigned 16-bit words.
		/// </summary>
		/// <remarks>
		/// Consider using one of the static helpers such as <see cref="CreateVolume(IDisplaySet)"/> to construct and automatically fill a <see cref="Volume"/>.
		/// </remarks>
		public Volume(ushort[] data, Size3D dimensions, Vector3D spacing, Vector3D originPatient,
		              Matrix orientationPatient, IDicomAttributeProvider dicomAttributeModel, int paddingValue)
			: this(null, data, dimensions, spacing, originPatient, orientationPatient,
			       VolumeSopDataSourcePrototype.Create(dicomAttributeModel), paddingValue) {}

		private Volume(short[] dataInt16, ushort[] dataUInt16, Size3D dimensions, Vector3D spacing, Vector3D originPatient,
		               Matrix orientationPatient, VolumeSopDataSourcePrototype sopDataSourcePrototype, int paddingValue)
		{
			Platform.CheckTrue(dataInt16 != null ^ dataUInt16 != null, "Exactly one of dataInt16 and dataUInt16 must be non-null.");
			_volShortArray = dataInt16;
			_volUnsignedShortArray = dataUInt16;

			_arrayDimensions = dimensions;
			_spacing = spacing;
			_originPatient = originPatient;
			_orientationPatientMatrix = orientationPatient;
			_modelDicom = sopDataSourcePrototype;
			_padValue = paddingValue;

			PersonName patientName = new PersonName(sopDataSourcePrototype[DicomTags.PatientsName].ToString());
			string patientId = sopDataSourcePrototype[DicomTags.PatientId].ToString();
			_description = string.Format("MPR {0} - {1}", patientName.FormattedName, patientId);
		}

		#endregion

		public string Description
		{
			get { return _description; }
		}

		#region Public properties

		/// <summary>
		/// The effective volume dimensions in Volume space.
		/// </summary>
		public Vector3D Dimensions
		{
			get
			{
				return new Vector3D(ArrayDimensions.Width * Spacing.X, ArrayDimensions.Height * Spacing.Y,
				                    ArrayDimensions.Depth * Spacing.Z);
			}
		}

		/// <summary>
		/// Spacing in millimeters along respective axes in Volume space.
		/// </summary>
		public Vector3D Spacing
		{
			get { return _spacing; }
		}

		/// <summary>
		/// The origin of the volume in Patient space.
		/// </summary>
		public Vector3D OriginPatient
		{
			get { return _originPatient; }
		}

		/// <summary>
		/// The orientation of the volume in Patient space.
		/// </summary>
		public Matrix OrientationPatientMatrix
		{
			get { return _orientationPatientMatrix; }
		}

		/// <summary>
		/// Indicates whether this volume contains signed or unsigned shorts
		/// </summary>
		/// <returns>true if unsigned, false if signed</returns>
		public bool IsDataUnsigned()
		{
			return _volUnsignedShortArray != null;
		}

		/// <summary>
		/// The number of voxels represented by this volume (and in the volume array).
		/// </summary>
		public int SizeInVoxels
		{
			get { return ArrayDimensions.Volume; }
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

		public float MinSpacing
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
				return (float)Math.Sqrt(Dimensions.X * Dimensions.X +
										 Dimensions.Y * Dimensions.Y +
										 Dimensions.Z * Dimensions.Z);
			}
		}

		/// <summary>
		/// Volume center point in volume coordinates
		/// </summary>
		public Vector3D CenterPoint
		{
			get
			{
				Vector3D center = new Vector3D(Origin.X + Spacing.X * 0.5f * ArrayDimensions.Width,
											   Origin.Y + Spacing.Y * 0.5f * ArrayDimensions.Height,
											   Origin.Z + Spacing.Z * 0.5f * ArrayDimensions.Depth);
				return center;
			}
		}

		/// <summary>
		/// Volume center point in patient coordinates
		/// </summary>
		public Vector3D CenterPointPatient
		{
			get { return ConvertToPatient(CenterPoint); }
		}

		public bool IsPointInVolume(Vector3D point)
		{
			return point.X >= MinXCoord && point.X <= MaxXCoord &&
			       point.Y >= MinYCoord && point.Y <= MaxYCoord &&
			       point.Z >= MinZCoord && point.Z <= MaxZCoord;
		}

		#endregion

		#region Coordinate Transforms

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
			Vector3D rotatedOrigin = RotateToVolumeOrientation(OriginPatient);
			patientVolumeTransform.SetRow(3, -rotatedOrigin.X, -rotatedOrigin.Y, -rotatedOrigin.Z, 1);

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
			Matrix volumePos = new Matrix(1, 4);
			volumePos.SetRow(0, volumeVec.X, volumeVec.Y, volumeVec.Z, 1F);
			Matrix patientPos = volumePos * OrientationPatientMatrix;
			return new Vector3D(patientPos[0, 0], patientPos[0, 1], patientPos[0, 2]);
		}

		public Vector3D RotateToVolumeOrientation(Vector3D patientVec)
		{
			Matrix patientPos = new Matrix(1, 4);
			patientPos.SetRow(0, patientVec.X, patientVec.Y, patientVec.Z, 1F);
			Matrix volumePos = patientPos * OrientationPatientMatrix.Transpose();
			return new Vector3D(volumePos[0, 0], volumePos[0, 1], volumePos[0, 2]);
		}

		#endregion

		#region Implementation

		internal IDicomAttributeProvider DataSet
		{
			get { return _modelDicom; }
		}

		// Decided to keep private for now, shouldn't be interesting to the outside world, and helps 
		//	avoid confusion with dimensions that take spacing into account (which is useful to the outside world)
		private Size3D ArrayDimensions
		{
			get { return _arrayDimensions; }
		}

		#region VTK volume wrapper

		private vtkImageData CreateVtkVolume()
		{
			vtkImageData vtkVolume = new vtkImageData();

			VtkHelper.RegisterVtkErrorEvents(vtkVolume);

			vtkVolume.SetDimensions(ArrayDimensions.Width, ArrayDimensions.Height, ArrayDimensions.Depth);
			vtkVolume.SetOrigin(Origin.X, Origin.Y, Origin.Z);
			vtkVolume.SetSpacing(Spacing.X, Spacing.Y, Spacing.Z);

			if (IsDataUnsigned())
			{
				vtkVolume.SetScalarTypeToUnsignedShort();
				vtkVolume.GetPointData().SetScalars(
					VtkHelper.ConvertToVtkUnsignedShortArray(_volUnsignedShortArray));
			}
			else
			{
				vtkVolume.SetScalarTypeToShort();
				vtkVolume.GetPointData().SetScalars(
					VtkHelper.ConvertToVtkShortArray(_volShortArray));
			}

			// This call is necessary to ensure vtkImageData data's info is correct (e.g. updates WholeExtent values)
			vtkVolume.UpdateInformation();

			return vtkVolume;
		}

		/// <summary>
		/// Call to obtain a VTK volume structure that is safe for VTK to operate on. When done
		/// operating on the volume, call <see cref="ReleasePinnedVtkVolume"/>.
		/// </summary>
		/// <returns></returns>
		//TODO: Wrap with disposable object and have released on Dispose
		internal vtkImageData ObtainPinnedVtkVolume()
		{
			// Create the VTK volume wrapper if it doesn't exist
			if (_cachedVtkVolume == null)
				_cachedVtkVolume = CreateVtkVolume();

			// Pin the managed volume array. If not null, then already pinned so we do not re-pin.
			if (_volArrayPinnedHandle == null)
			{
				if (IsDataUnsigned())
					_volArrayPinnedHandle = GCHandle.Alloc(_volUnsignedShortArray, GCHandleType.Pinned);
				else
					_volArrayPinnedHandle = GCHandle.Alloc(_volShortArray, GCHandleType.Pinned);
			}

			return _cachedVtkVolume;
		}

		internal void ReleasePinnedVtkVolume()
		{
			if (_volArrayPinnedHandle != null)
			{
				// Check for null avoids calling this twice
				_volArrayPinnedHandle.Value.Free();
				_volArrayPinnedHandle = null;
			}
		}

		#endregion

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
				// This should have been taken care of by caller of Obtain, release here just to be safe.
				ReleasePinnedVtkVolume();

				if (_cachedVtkVolume != null)
				{
					_cachedVtkVolume.GetPointData().Dispose();
					_cachedVtkVolume.Dispose();
					_cachedVtkVolume = null;
				}

				_volShortArray = null;
				_volUnsignedShortArray = null;
			}
		}

		#endregion
	}
}