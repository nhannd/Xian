#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Utilities
{
	public static class Math3D
	{
		public static Matrix OrientationMatrixFromVectors(Vector3D xVec, Vector3D yVec, Vector3D zVec)
		{
			return new Matrix
				(new float[4,4]
				 	{
				 		{xVec.X, xVec.Y, xVec.Z, 0},
				 		{yVec.X, yVec.Y, yVec.Z, 0},
				 		{zVec.X, zVec.Y, zVec.Z, 0},
				 		{0, 0, 0, 1}
				 	});
		}
	}
}