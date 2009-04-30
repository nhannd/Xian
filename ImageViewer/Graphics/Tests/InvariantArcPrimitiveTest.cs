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

using NUnit.Framework;
using System.Drawing;
using System;
using ClearCanvas.ImageViewer.Mathematics;

namespace ClearCanvas.ImageViewer.Graphics.Tests
{
	[TestFixture]
	public class InvariantArcPrimitiveTest
	{
		public InvariantArcPrimitiveTest()
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
		public void CoordinateChange()
		{
			InvariantArcPrimitive arc = new InvariantArcPrimitive();

			for (int angle = 0; angle <= 360; angle += 90)
			{
				arc.SpatialTransform.RotationXY = angle;
				arc.SpatialTransform.FlipX = true;
				arc.SpatialTransform.FlipY = true;
				arc.CoordinateSystem = CoordinateSystem.Source;
				arc.StartAngle = 30;
				arc.CoordinateSystem = CoordinateSystem.Destination;
				float dstAngle = arc.StartAngle;
				arc.StartAngle = dstAngle;
				arc.CoordinateSystem = CoordinateSystem.Source;
				Assert.AreEqual(30, arc.StartAngle);
			}
		}
	}
}

#endif