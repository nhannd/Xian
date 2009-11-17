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