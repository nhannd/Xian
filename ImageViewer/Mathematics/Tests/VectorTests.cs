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
using System.Drawing;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Mathematics.Tests
{
	[TestFixture]
	public class VectorTests
	{
		public VectorTests()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void SubtendedAngle()
		{
			double angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(1, 0));
			Assert.AreEqual(0, angle);

			angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(0, 1));
			Assert.AreEqual(-90, angle);

			angle = Vector.SubtendedAngle(new PointF(0, 10), new PointF(0, 0), new PointF(1, 0));
			Assert.AreEqual(90, angle);

			angle = Vector.SubtendedAngle(new PointF(10, 0), new PointF(0, 0), new PointF(-1, 0));
			Assert.AreEqual(180, angle);
		}

		[Test]
		public void UnitVector()
		{
			float oneOverSqrt2 = 1/(float) Math.Sqrt(2);
			SizeF unitVector;
			PointF origin;

			origin = new PointF(0, 0);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-1, 1)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(-1, 1);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(3, -1);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");

			origin = new PointF(3, 1);
			unitVector = new SizeF(Vector.CreateUnitVector(origin, origin + new SizeF(-2, 2)));
			Assert.IsTrue(FloatComparer.AreEqual(new SizeF(-oneOverSqrt2, oneOverSqrt2), unitVector));
			Assert.IsTrue(FloatComparer.AreEqual(unitVector.Width*unitVector.Width + unitVector.Height*unitVector.Height, 1), "Magnitude must be 1.");
		}
	}
}

#endif