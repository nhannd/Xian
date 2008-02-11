using System.Drawing;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.Mathematics;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Tools.Synchronization
{
	internal static class ImagePositionHelper
	{
		/// <summary>
		/// Calculates the position vector (in patient space) of the top left corner of an image.
		/// </summary>
		public static Vector3D SourceToPatientTopLeftOfImage(ImageSop sop)
		{
			Platform.CheckForNullReference(sop, "sop");
			return SourceToPatient(sop.ImageOrientationPatient, sop.PixelSpacing, sop.ImagePositionPatient, 0F, 0F);
		}

		/// <summary>
		/// Calculates the position vector (in patient space) of the center of an image.
		/// </summary>
		public static Vector3D SourceToPatientCenterOfImage(ImageSop sop)
		{
			Platform.CheckForNullReference(sop, "sop");
			return SourceToPatient(sop.ImageOrientationPatient, sop.PixelSpacing, sop.ImagePositionPatient, (sop.Columns - 1)/2F, (sop.Rows - 1)/2F);
		}

		/// <summary>
		/// Calculates the position vector (in patient space) of the bottom right corner of an image.
		/// </summary>
		public static Vector3D SourceToPatientBottomRightOfImage(ImageSop sop)
		{
			Platform.CheckForNullReference(sop, "sop");
			return SourceToPatient(sop.ImageOrientationPatient, sop.PixelSpacing, sop.ImagePositionPatient, sop.Columns - 1, sop.Rows - 1);
		}

		internal static Vector3D SourceToPatient(ImageSop sop, PointF sourcePoint)
		{
			return SourceToPatient(sop, sourcePoint.X, sourcePoint.Y);
		}

		public static Vector3D SourceToPatient(ImageSop sop, float pixelPositionX, float pixelPositionY)
		{
			return SourceToPatient(sop.ImageOrientationPatient, sop.PixelSpacing, sop.ImagePositionPatient, pixelPositionX, pixelPositionY);
		}

		public static Vector3D SourceToPatient(
			ImageOrientationPatient orientation, 
			PixelSpacing pixelSpacing, 
			ImagePositionPatient position,
			float pixelPositionX,
			float pixelPositionY)
		{
			Platform.CheckForNullReference(orientation, "orientation");
			Platform.CheckForNullReference(pixelSpacing, "pixelSpacing");
			Platform.CheckForNullReference(position, "position");

			if (orientation.IsNull || pixelSpacing.IsNull)
				return null;

			if (pixelPositionX == 0F && pixelPositionY == 0F)
				return new Vector3D((float)position.X, (float)position.Y, (float)position.Z);

			// Calculation of position of the center of the image in patient coordinates 
			// using the matrix method described in Dicom PS 3.3 C.7.6.2.1.1.

			Matrix mReference = new Matrix(4, 4);

			mReference.SetColumn(0, (float)(orientation.RowX * pixelSpacing.Column), 
									(float)(orientation.RowY * pixelSpacing.Column), 
									(float)(orientation.RowZ * pixelSpacing.Column), 0F);

			mReference.SetColumn(1, (float)(orientation.ColumnX * pixelSpacing.Row),
									(float)(orientation.ColumnY * pixelSpacing.Row),
									(float)(orientation.ColumnZ * pixelSpacing.Row), 0F);

			mReference.SetColumn(3, (float)position.X, (float)position.Y, (float)position.Z, 1F);

			Matrix columnMatrix = new Matrix(4, 1);
			columnMatrix.SetColumn(0, pixelPositionX, pixelPositionY, 0F, 1F);

			Matrix result = mReference * columnMatrix;

			return new Vector3D(result[0, 0], result[1, 0], result[2, 0]);
		}

		public static Vector3D CalculateNormalVector(ImageSop sop)
		{
			Platform.CheckForNullReference(sop, "sop");
			
			return CalculateNormalVector(sop.ImageOrientationPatient);
		}

		public static Vector3D CalculateNormalVector(ImageOrientationPatient orientation)
		{
			Platform.CheckForNullReference(orientation, "orientation");

			if (orientation.IsNull)
				return null;

			Vector3D left = new Vector3D((float) orientation.RowX, (float) orientation.RowY, (float) orientation.RowZ);
			return left.Cross(new Vector3D((float)orientation.ColumnX, (float)orientation.ColumnY, (float)orientation.ColumnZ));
		}

		public static Matrix GetRotationMatrix(ImageSop sop)
		{
			Platform.CheckForNullReference(sop, "sop");
			return GetRotationMatrix(sop.ImageOrientationPatient, sop.ImagePositionPatient);
		}

		public static Matrix GetRotationMatrix(ImageOrientationPatient orientation, ImagePositionPatient position)
		{
			Platform.CheckForNullReference(orientation, "orientation");
			Platform.CheckForNullReference(position, "position");
			if (orientation.IsNull)
				return null;

			Vector3D normal = CalculateNormalVector(orientation);

			Matrix transform = new Matrix(3, 3);
			transform.SetRow(0, (float)orientation.RowX, (float)orientation.RowY, (float)orientation.RowZ);
			transform.SetRow(1, (float)orientation.ColumnX, (float)orientation.ColumnY, (float)orientation.ColumnZ);
			transform.SetRow(2, normal.X, normal.Y, normal.Z);
			return transform;
		}
	}
}
