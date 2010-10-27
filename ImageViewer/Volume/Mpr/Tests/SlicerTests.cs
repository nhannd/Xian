#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using ClearCanvas.ImageViewer.Mathematics;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Volume.Mpr.Tests
{
	[TestFixture]
	public class SlicerTests
	{
		public SlicerTests() {}

		[Test]
		public void TestInverseRotationMatrices()
		{
			for (int x = 0; x < 360; x += 15)
			{
				for (int y = 0; y < 360; y += 15)
				{
					for (int z = 0; z < 360; z += 15)
					{
						VolumeSlicerParams expected = new VolumeSlicerParams(x, y, z); // this constructor stores x, y, z separately from the matrix
						Matrix mExpected = ComputeRotationMatrix(expected.RotateAboutX, expected.RotateAboutY, expected.RotateAboutZ);

						VolumeSlicerParams test = new VolumeSlicerParams(expected.SlicingPlaneRotation); // this constructor must infer x, y, z from the matrix
						Matrix mTest = ComputeRotationMatrix(test.RotateAboutX, test.RotateAboutY, test.RotateAboutZ);

						for (int r = 0; r < 3; r++)
						{
							for (int c = 0; c < 3; c++)
							{
								Assert.AreEqual(mExpected[r, c], mTest[r, c], 1e-6, "Rotation Matrices differ at R{3}C{4} for Q={0},{1},{2}", x, y, z, r, c);
							}
						}
					}
				}
			}
		}

		private static Matrix ComputeRotationMatrix(float degreesAboutX, float degreesAboutY, float degreesAboutZ)
		{
			Matrix mTestX = Matrix.GetIdentity(3);
			mTestX[1, 1] = mTestX[2, 2] = Cos(degreesAboutX);
			mTestX[1, 2] = -(mTestX[2, 1] = Sin(degreesAboutX));

			Matrix mTestY = Matrix.GetIdentity(3);
			mTestY[0, 0] = mTestY[2, 2] = Cos(degreesAboutY);
			mTestY[2, 0] = -(mTestY[0, 2] = Sin(degreesAboutY));

			Matrix mTestZ = Matrix.GetIdentity(3);
			mTestZ[0, 0] = mTestZ[1, 1] = Cos(degreesAboutZ);
			mTestZ[0, 1] = -(mTestZ[1, 0] = Sin(degreesAboutZ));

			return mTestX*mTestY*mTestZ;
		}

		private static float Cos(float degrees)
		{
			return (float) Math.Cos(degrees*Math.PI/180);
		}

		private static float Sin(float degrees)
		{
			return (float) Math.Sin(degrees*Math.PI/180);
		}
	}
}

#endif