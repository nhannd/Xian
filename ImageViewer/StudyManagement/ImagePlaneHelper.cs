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

using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	/// <summary>
	/// Contains helper methods for converting between image coordinates
	/// and the patient coordinate system defined by the Dicom Image Plane Module.
	/// </summary>
	public class ImagePlaneHelper
	{
		private readonly Frame _frame;

		// No sense recalculating these things since they never change.
		private Vector3D _imagePositionPatient;
		private Vector3D _normalVector;
		private Matrix _rotationMatrix;
		private Matrix _pixelToPatientTransform;

		internal ImagePlaneHelper(Frame frame)
		{
			Platform.CheckForNullReference(frame, "frame");
			_frame = frame;
		}

		private Vector3D ImagePositionPatient
		{
			get
			{
				if (_imagePositionPatient == null)
				{
					ImagePositionPatient position = _frame.ImagePositionPatient;
					_imagePositionPatient = new Vector3D((float)position.X, (float)position.Y, (float)position.Z);
				}

				return _imagePositionPatient;
			}
		}

		/// <summary>
		/// Converts the input image position (expressed in pixels) to the patient coordinate system.
		/// </summary>
		/// <returns>A position vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D ConvertToPatient(PointF positionPixels)
		{
			ImageOrientationPatient orientation = _frame.ImageOrientationPatient;
			PixelSpacing pixelSpacing = _frame.PixelSpacing;

			if (orientation.IsNull || pixelSpacing.IsNull)
				return null;

			Vector3D position = this.ImagePositionPatient;
			// A shortcut for when the pixel position is (0, 0).
			if (positionPixels.X == 0F && positionPixels.Y == 0F)
				return position;

			// Calculation of position in patient coordinates using 
			// the matrix method described in Dicom PS 3.3 C.7.6.2.1.1.

			if (_pixelToPatientTransform == null)
			{
				_pixelToPatientTransform = new Matrix(4, 4);

				_pixelToPatientTransform.SetColumn(0, (float)(orientation.RowX * pixelSpacing.Column),
									 (float)(orientation.RowY * pixelSpacing.Column),
									 (float)(orientation.RowZ * pixelSpacing.Column), 0F);

				_pixelToPatientTransform.SetColumn(1, (float)(orientation.ColumnX * pixelSpacing.Row),
									 (float)(orientation.ColumnY * pixelSpacing.Row),
									 (float)(orientation.ColumnZ * pixelSpacing.Row), 0F);

				_pixelToPatientTransform.SetColumn(3, position.X, position.Y, position.Z, 1F);
			}

			Matrix columnMatrix = new Matrix(4, 1);
			columnMatrix.SetColumn(0, positionPixels.X, positionPixels.Y, 0F, 1F);
			Matrix result = _pixelToPatientTransform * columnMatrix;

			return new Vector3D(result[0, 0], result[1, 0], result[2, 0]);
		}

		/// <summary>
		/// Converts the input <paramref name="positionPatient">position vector</paramref> to the coordinate
		/// system of the image plane, moving the origin to <paramref name="originPatient"/>.
		/// </summary>
		/// <remarks>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </remarks>
		/// <param name="positionPatient">The position vector, in patient coordinates,
		/// to be converted to the coordinate system of the image plane.</param>
		/// <param name="originPatient">The new origin, in patient coordinates.</param>
		/// <returns>A position vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImagePlane(Vector3D positionPatient, Vector3D originPatient)
		{
			Platform.CheckForNullReference(positionPatient, "positionPatient");

			Vector3D translated = positionPatient;
			if (originPatient != null)
				translated -= originPatient;

			Matrix rotationMatrix = GetRotationMatrix();
			if (rotationMatrix == null)
				return null;

			Matrix translatedMatrix = new Matrix(3, 1);
			translatedMatrix.SetColumn(0, translated.X, translated.Y, translated.Z);

			// Rotate coordinate system to match that of the image plane.
			Matrix rotated = rotationMatrix * translatedMatrix;
			return new Vector3D(rotated[0, 0], rotated[1, 0], rotated[2, 0]);
		}

		/// <summary>
		/// Converts the input <paramref name="positionPatient">position vector</paramref> to the coordinate
		/// system of the image plane, moving the origin to the top left corner of the image.
		/// </summary>
		/// <remarks>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </remarks>
		/// <param name="positionPatient">The position vector, in patient coordinates,
		/// to be converted to the coordinate system of the image plane.</param>
		/// <returns>A position vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImagePlane(Vector3D positionPatient)
		{
			Platform.CheckForNullReference(positionPatient, "positionPatient");
			return ConvertToImagePlane(positionPatient, this.ImagePositionPatient);
		}

		/// <summary>
		/// Converts a point in the image plane expressed in millimetres (mm) into a point expressed in pixels.
		/// </summary>
		/// <returns>The corresponding pixel coordinate, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public PointF? ConvertToImage(PointF positionMillimetres)
		{
			PixelSpacing spacing = _frame.PixelSpacing;
			if (spacing.IsNull)
				return null;

			return new PointF(positionMillimetres.X / (float)spacing.Column, positionMillimetres.Y / (float)spacing.Row);
		}

		/// <summary>
		/// Gets the normal vector describing the plane of the image in patient coordinates.
		/// </summary>
		/// <returns>The normal vector, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		public Vector3D GetNormalVector()
		{
			ImageOrientationPatient orientation = _frame.ImageOrientationPatient;
			if (orientation.IsNull)
				return null;

			if (_normalVector == null)
			{
				Vector3D left = new Vector3D((float)orientation.RowX, (float)orientation.RowY, (float)orientation.RowZ);
				_normalVector = left.Cross(new Vector3D((float)orientation.ColumnX, (float)orientation.ColumnY, (float)orientation.ColumnZ));
			}

			return _normalVector;
		}

		/// <summary>
		/// Gets a rotation matrix that, when multiplied by a column matrix representing a
		/// position vector in patient coordinates, will rotate the position vector
		/// into a coordinate system matching that of the image plane.
		/// </summary>
		/// <returns>The rotation matrix, or null if the <see cref="Frame"/>'s position information is invalid.</returns>
		private Matrix GetRotationMatrix()
		{
			if (_rotationMatrix == null)
			{
				ImageOrientationPatient orientation = _frame.ImageOrientationPatient;
				if (orientation.IsNull)
					return null;

				Vector3D normal = GetNormalVector();

				_rotationMatrix = new Matrix(3, 3);
				_rotationMatrix.SetRow(0, (float)orientation.RowX, (float)orientation.RowY, (float)orientation.RowZ);
				_rotationMatrix.SetRow(1, (float)orientation.ColumnX, (float)orientation.ColumnY, (float)orientation.ColumnZ);
				_rotationMatrix.SetRow(2, normal.X, normal.Y, normal.Z);
			}

			return _rotationMatrix;
		}
	}
}
