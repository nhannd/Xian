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

		private readonly vtkImageData _vtkImageData;
		private readonly bool _dataUnsigned;
		private readonly short[] _volShortArray;
		private readonly ushort[] _volUnsignedShortArray;

		private readonly int _width;
		private readonly int _height;
		private readonly int _depth;

		private readonly float _originX;
		private readonly float _originY;
		private readonly float _originZ;

		private readonly int _wholeExtentX;
		private readonly int _wholeExtentY;
		private readonly int _wholeExtentZ;

		private readonly float _spacingX;
		private readonly float _spacingY;
		private readonly float _spacingZ;

		private readonly Matrix _dicomOrientationPatientMatrix;

		private readonly DicomMessageBase _modelDicom;

		private readonly bool _instanceAndSliceLocationReversed;

		// This handle is used to pin the volume array
		private GCHandle _volArrayPinnedHandle;

		#endregion

		#region Initialization

		public Volume(short[] volShortArray, vtkImageData imageData, Matrix volumeOrientation, DicomMessageBase modelDicom, bool instanceAndSliceLocationReversed)
			:
			this(imageData, volumeOrientation, false, modelDicom, instanceAndSliceLocationReversed)
		{
			_volShortArray = volShortArray;
			//ggerade ToOpt: I think it would be better to pin these only while necessary. Consider refactoring
			//	such that getting vtkImageData pins, then a call to release
			_volArrayPinnedHandle = GCHandle.Alloc(_volShortArray, GCHandleType.Pinned);
		}

		public Volume(ushort[] volUnsignedShortArray, vtkImageData imageData, Matrix volumeOrientation, DicomMessageBase modelDicom, bool instanceAndSliceLocationReversed)
			:
			this(imageData, volumeOrientation, true, modelDicom, instanceAndSliceLocationReversed)
		{
			_volUnsignedShortArray = volUnsignedShortArray;
			_volArrayPinnedHandle = GCHandle.Alloc(_volUnsignedShortArray, GCHandleType.Pinned);
		}

		private Volume(vtkImageData imageData, Matrix volumeOrientation, bool dataUnsigned, DicomMessageBase modelDicom, bool instanceAndSliceLocationReversed)
		{
			_vtkImageData = imageData;
			_dicomOrientationPatientMatrix = volumeOrientation;
			_dataUnsigned = dataUnsigned;

			int[] dimensions = _vtkImageData.GetDimensions();
			_width = dimensions[0];
			_height = dimensions[1];
			_depth = dimensions[2];

			double[] spacing = _vtkImageData.GetSpacing();
			_spacingX = (float)spacing[0];
			_spacingY = (float)spacing[1];
			_spacingZ = (float)spacing[2];

			double[] origin = _vtkImageData.GetOrigin();
			_originX = (float)origin[0];
			_originY = (float)origin[1];
			_originZ = (float)origin[2];

			int[] wholeExtent = _vtkImageData.GetWholeExtent();
			_wholeExtentX = wholeExtent[0] + wholeExtent[1];
			_wholeExtentY = wholeExtent[2] + wholeExtent[3];
			_wholeExtentZ = wholeExtent[4] + wholeExtent[5];

			_modelDicom = modelDicom;

			_instanceAndSliceLocationReversed = instanceAndSliceLocationReversed;
		}

		internal Volume(short[] volShortArray, bool dataUnsigned, int width, int height, int depth,
			float spacingX, float spacingY, float spacingZ,
			float originX, float originY, float originZ,
			int wholeExtentX, int wholeExtentY, int wholeExtentZ,
			DicomMessageBase modelDicom
			)
		{
			_volShortArray = volShortArray;
			_dataUnsigned = dataUnsigned;
			_width = width;
			_height = height;
			_depth = depth;
			_spacingX = spacingX;
			_spacingY = spacingY;
			_spacingZ = spacingZ;
			_originX = originX;
			_originY = originY;
			_originZ = originZ;
			_wholeExtentX = wholeExtentX;
			_wholeExtentY = wholeExtentY;
			_wholeExtentZ = wholeExtentZ;
			_modelDicom = modelDicom;
		}
		
		#endregion

		#region Public properties

		public vtkImageData _VtkImageData
		{
			get { return _vtkImageData; }
		}

		public Matrix DicomOrientationPatientMatrix
		{
			get { return _dicomOrientationPatientMatrix; }
		}

		public int Width
		{
			get { return _width; }
		}

		public int Height
		{
			get { return _height; }
		}

		public int Depth
		{
			get { return _depth; }
		}

		public float OriginX
		{
			get { return _originX; }
		}

		public float OriginY
		{
			get { return _originY; }
		}

		public float OriginZ
		{
			get { return _originZ; }
		}

		public int SizeInVoxels
		{
			get { return Width * Height * Depth; }
		}

		public float SpacingX
		{
			get { return _spacingX; }
		}

		public float SpacingY
		{
			get { return _spacingY; }
		}

		public float SpacingZ
		{
			get { return _spacingZ; }
		}

		public DicomMessageBase _ModelDicom
		{
			get { return _modelDicom; }
		}

		public bool IsDataUnsigned()
		{
			return _dataUnsigned;
		}

		public float MinXCoord
		{
			get { return OriginX; }
		}

		public float MaxXCoord
		{
			get { return (OriginX + _spacingX * _wholeExtentX); }
		}

		public float MinYCoord
		{
			get { return OriginY; }
		}

		public float MaxYCoord
		{
			get { return (OriginY + _spacingY * _wholeExtentY); }
		}

		public float MinZCoord
		{
			get { return OriginZ; }
		}

		public float MaxZCoord
		{
			get { return (OriginZ + _spacingZ * _wholeExtentZ); }
		}

		public bool InstanceAndSliceLocationReversed
		{
			get { return _instanceAndSliceLocationReversed; }
		}

		#endregion

		#region Public methods

		public float[] CalcCenterPoint()
		{
			// Volume center point
			float[] center = new float[3];
			center[0] = OriginX + _spacingX * 0.5f * _wholeExtentX;
			center[1] = OriginY + _spacingY * 0.5f * _wholeExtentY;
			center[2] = OriginZ + _spacingZ * 0.5f * _wholeExtentZ;
			return center;
		}

		#endregion

		#region Disposal

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) 
			{
				_volArrayPinnedHandle.Free();
				if (_vtkImageData != null)
					_vtkImageData.Dispose();
			}
		}

		#endregion

	}
}
