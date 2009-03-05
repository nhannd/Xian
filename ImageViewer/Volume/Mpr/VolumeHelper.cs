using System;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr
{
	internal class VolumeHelper
	{
		public static Matrix OrientationMatrixFromVectors(Vector3D xVec, Vector3D yVec, Vector3D zVec)
		{
			return new Matrix
				(4, 4, new float[4, 4]
				       	{
				       		{xVec.X, xVec.Y, xVec.Z, 0},
				       		{yVec.X, yVec.Y, yVec.Z, 0},
				       		{zVec.X, zVec.Y, zVec.Z, 0},
				       		{0, 0, 0, 1}
				       	});
		}

		public static bool EqualsWithinTolerance(double d1, double d2, float tolerance)
		{
			return Math.Abs(d1 - d2) < tolerance;
		}
	}
}
