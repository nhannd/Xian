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

namespace ClearCanvas.ImageViewer.Mathematics.Tests {
	[TestFixture]
	public class PolygonFTests {
		public PolygonFTests() {
		}

		[TestFixtureSetUp]
		public void Init() {
		}

		[TestFixtureTearDown]
		public void Cleanup() {
		}

		[Test]
		public void Contains() {
			PolygonF polygon = new PolygonF(new PointF[] {new PointF(0,0), new PointF(1,0), new PointF(1,1), new PointF(0, 1) });
			Assert.IsTrue(polygon.Contains(new PointF(0.5f, 0.5f))); // inside
			Assert.IsFalse(polygon.Contains(new PointF(0.5f, 1.5f))); // above
			Assert.IsFalse(polygon.Contains(new PointF(0f, 1.5f))); // above

			Assert.IsTrue(polygon.Contains(new PointF(0f, 0.5f))); // left edge
			Assert.IsFalse(polygon.Contains(new PointF(1f, 0.5f))); // right edge
			Assert.IsTrue(polygon.Contains(new PointF(0.5f, 0f))); // bottom edge
			Assert.IsFalse(polygon.Contains(new PointF(0.5f, 1f))); // top edge

			Assert.IsTrue(polygon.Contains(new PointF(0f, 0f))); // bottom left corner
			Assert.IsFalse(polygon.Contains(new PointF(0f, 1f))); // top left corner
			Assert.IsFalse(polygon.Contains(new PointF(1f, 0f))); // bottom right corner
			Assert.IsFalse(polygon.Contains(new PointF(1f, 1f))); // top right corner
		}
	}
}

#endif