using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
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
		private Vector3D _normalVector;
		private Matrix _rotationMatrix;
		private Matrix _pixelToPatientTransform;

		internal ImagePlaneHelper(Frame frame)
		{
			Platform.CheckForNullReference(frame, "frame");
			_frame = frame;
		}

		/// <summary>
		/// Converts the input image position (expressed in pixels) to the patient coordinate system.
		/// </summary>
		/// <returns>A position vector, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public Vector3D ConvertToPatient(PointF positionPixels)
		{
			ImageOrientationPatient orientation = _frame.ImageOrientationPatient;
			PixelSpacing pixelSpacing = _frame.PixelSpacing;

			if (orientation.IsNull || pixelSpacing.IsNull)
				return null;

			ImagePositionPatient position = _frame.ImagePositionPatient;

			// A shortcut for when the pixel position is (0, 0).
			if (positionPixels.X == 0F && positionPixels.Y == 0F)
				return new Vector3D((float)position.X, (float)position.Y, (float)position.Z);

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

				_pixelToPatientTransform.SetColumn(3, (float)position.X, (float)position.Y, (float)position.Z, 1F);
			}

			Matrix columnMatrix = new Matrix(4, 1);
			columnMatrix.SetColumn(0, positionPixels.X, positionPixels.Y, 0F, 1F);
			Matrix result = _pixelToPatientTransform * columnMatrix;

			return new Vector3D(result[0, 0], result[1, 0], result[2, 0]);
		}

		/// <summary>
		/// Converts the input position vector (in patient coordinates) to the coordinate
		/// system of the image, with the origin at pixel position (0, 0).
		/// </summary>
		/// <remarks>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </remarks>
		/// <returns>A position vector, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImage(Vector3D positionPatient)
		{
			return ConvertToImage(positionPatient, PointF.Empty);
		}

		/// <summary>
		/// Converts the input position vector (in patient coordinates) to the coordinate
		/// system of the image, with the origin at <paramref name="originPixels"/>.
		/// </summary>
		/// <remarks>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </remarks>
		/// <returns>A position vector, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImage(Vector3D positionPatient, PointF originPixels)
		{
			Platform.CheckForNullReference(positionPatient, "positionPatient");

			Vector3D originPatient = ConvertToPatient(originPixels);
			if (originPatient == null)
				return null;

			return ConvertToImage(positionPatient, originPatient);
		}

		/// <summary>
		/// Converts the input position vector (in patient coordinates) to the coordinate
		/// system of the image, with the origin at <paramref name="originPatient"/> (in patient coordinates).
		/// </summary>
		/// <remarks>
		/// <para>
		/// Note that the resultant position vector remains in units of mm and the z-coordinate is valid.
		/// </para>
		/// When <paramref name="originPatient"/> is a zero vector, a simple rotation is performed.
		/// This means that the origin remains unchanged (e.g. it stays at (0, 0, 0) in the patient coordinate system),
		/// but the coordinate system is rotated about the origin to match the plane of the image.
		/// </remarks>
		/// <returns>A position vector, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public Vector3D ConvertToImage(Vector3D positionPatient, Vector3D originPatient)
		{
			Platform.CheckForNullReference(positionPatient, "positionPatient");
			Platform.CheckForNullReference(originPatient, "originPatient");

			Matrix rotationMatrix = GetRotationMatrix();
			if (rotationMatrix == null)
				return null;

			// Translate to the origin (in patient coordinates).
			Vector3D translated = positionPatient - originPatient;

			Matrix translatedMatrix = new Matrix(3, 1);
			translatedMatrix.SetColumn(0, translated.X, translated.Y, translated.Z);

			// Rotate coordinate system to match that of the image plane.
			Matrix rotated = GetRotationMatrix() * translatedMatrix;

			return new Vector3D(rotated[0, 0], rotated[1, 0], rotated[2, 0]);
		}

		/// <summary>
		/// Converts a point in the image plane expressed in pixels into a point expressed in millimetres (mm).
		/// </summary>
		/// <returns>The corresponding image coordinate, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public PointF? ConvertToImage(PointF positionPixels)
		{
			PixelSpacing spacing = _frame.PixelSpacing;
			if (spacing.IsNull)
				return null;

			return new PointF(positionPixels.X * (float)spacing.Column, positionPixels.Y * (float)spacing.Row);
		}

		/// <summary>
		/// Converts a point in the image plane expressed in millimetres (mm) into a point expressed in pixels.
		/// </summary>
		/// <returns>The corresponding pixel coordinate, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public PointF? ConvertToImagePixel(PointF positionMillimetres)
		{
			PixelSpacing spacing = _frame.PixelSpacing;
			if (spacing.IsNull)
				return null;

			return new PointF(positionMillimetres.X / (float)spacing.Column, positionMillimetres.Y / (float)spacing.Row);
		}

		/// <summary>
		/// Gets the normal vector describing the plane of the image in patient coordinates.
		/// </summary>
		/// <returns>The normal vector, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
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
		/// <returns>The rotation matrix, or null if the <see cref="ImageSop"/>'s position information is invalid.</returns>
		public Matrix GetRotationMatrix()
		{
			ImageOrientationPatient orientation = _frame.ImageOrientationPatient;
			if (orientation.IsNull)
				return null;

			if (_rotationMatrix == null)
			{
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
