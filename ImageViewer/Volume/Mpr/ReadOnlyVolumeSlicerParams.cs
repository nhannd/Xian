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
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	partial class VolumeSlicerParams
	{
		private class ReadOnlyVolumeSlicerParams : IVolumeSlicerParams
		{
			private const string _readOnly = "Cannot modify a read-only IVolumeSlicerParams.";

			private readonly string _description;
			private readonly Matrix _slicingPlaneRotation;
			private readonly Vector3D _sliceThroughPointPatient;
			private readonly VolumeSlicerInterpolationMode _interpolationMode;
			private readonly float _sliceExtentXMillimeters;
			private readonly float _sliceExtentYMillimeters;
			private readonly float _sliceSpacing;

			public ReadOnlyVolumeSlicerParams(IVolumeSlicerParams source)
			{
				if (source.SlicingPlaneRotation != null)
					_slicingPlaneRotation = new Matrix(source.SlicingPlaneRotation);
				if (source.SliceThroughPointPatient != null)
					_sliceThroughPointPatient = new Vector3D(source.SliceThroughPointPatient);
				_description = source.Description;
				_interpolationMode = source.InterpolationMode;
				_sliceExtentXMillimeters = source.SliceExtentXMillimeters;
				_sliceExtentYMillimeters = source.SliceExtentYMillimeters;
				_sliceSpacing = source.SliceSpacing;
			}

			public string Description
			{
				get { return _description; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public Matrix SlicingPlaneRotation
			{
				get
				{
					if (_slicingPlaneRotation == null)
						return null;
					return new Matrix(_slicingPlaneRotation);
				}
				set { throw new NotSupportedException(_readOnly); }
			}

			public Vector3D SliceThroughPointPatient
			{
				get
				{
					if (_sliceThroughPointPatient == null)
						return null;
					return new Vector3D(_sliceThroughPointPatient);
				}
				set { throw new NotSupportedException(_readOnly); }
			}

			public VolumeSlicerInterpolationMode InterpolationMode
			{
				get { return _interpolationMode; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public float SliceExtentXMillimeters
			{
				get { return _sliceExtentXMillimeters; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public float SliceExtentYMillimeters
			{
				get { return _sliceExtentYMillimeters; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public float SliceSpacing
			{
				get { return _sliceSpacing; }
				set { throw new NotSupportedException(_readOnly); }
			}

			public bool IsReadOnly
			{
				get { return true; }
			}
		}
	}
}