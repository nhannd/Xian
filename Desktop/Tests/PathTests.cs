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

#if UNIT_TESTS
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Utilities;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class PathTests
	{
		[Test]
		public void Test_Constructor_Literals()
		{
			var p = new Path("A/B/C");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("A", p.Segments[0].ResourceKey);
			Assert.AreEqual("A", p.Segments[0].LocalizedText);
			Assert.AreEqual("B", p.Segments[1].ResourceKey);
			Assert.AreEqual("B", p.Segments[1].LocalizedText);
			Assert.AreEqual("C", p.Segments[2].ResourceKey);
			Assert.AreEqual("C", p.Segments[2].LocalizedText);

			// test other constructor, with a null resolver
			p = new Path("A/B/C", null);
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("A", p.Segments[0].ResourceKey);
			Assert.AreEqual("A", p.Segments[0].LocalizedText);
			Assert.AreEqual("B", p.Segments[1].ResourceKey);
			Assert.AreEqual("B", p.Segments[1].LocalizedText);
			Assert.AreEqual("C", p.Segments[2].ResourceKey);
			Assert.AreEqual("C", p.Segments[2].LocalizedText);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor1_Null()
		{
			new Path((string)null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor2_Null()
		{
			new Path(null, new ResourceResolver(this.GetType(), false));
		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor3_Null()
		{
			new Path((PathSegment)null);
		}

		[Test]
		public void Test_Constructor1_EmptyPath()
		{
			var p = new Path("");
			Assert.AreEqual(0, p.Segments.Count);
			Assert.AreEqual(Path.Empty, p);
		}

		[Test]
		public void Test_Constructor2_EmptyPath()
		{
			var p = new Path("", null);
			Assert.AreEqual(0, p.Segments.Count);
			Assert.AreEqual(Path.Empty, p);
		}

		[Test]
		public void Test_Constructor_SingleSegment()
		{
			var p = new Path("a");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a", p.LastSegment.ResourceKey);
			Assert.AreEqual("a", p.LastSegment.LocalizedText);
		}

		[Test]
		public void Test_Constructor_PrecedingSeparator()
		{
			var p = new Path("/a");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a", p.LastSegment.ResourceKey);
			Assert.AreEqual("a", p.LastSegment.LocalizedText);
		}

		[Test]
		public void Test_EmptySegments()
		{
			var p = new Path("a//b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/b", p.ToString());

			p = new Path("a///b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/b", p.ToString());

			p = new Path("/a///b/");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/b", p.ToString());
		}

		[Test]
		public void Test_TrailingSeparator()
		{
			var p = new Path("a/");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a", p.LastSegment.ResourceKey);
			Assert.AreEqual("a", p.LastSegment.LocalizedText);
		}

		[Test]
		public void Test_EscapedSeparator()
		{
			var p = new Path("a'/b");
			Assert.AreEqual(1, p.Segments.Count);
			Assert.AreEqual("a/b", p.Segments[0].ResourceKey);

			p = new Path("a'//b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a/", p.Segments[0].ResourceKey);
			Assert.AreEqual("b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a'//b", p.ToString());

			p = new Path("a/'//b");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("/", p.Segments[1].ResourceKey);
			Assert.AreEqual("b", p.Segments[2].ResourceKey);
			Assert.AreEqual("a/'//b", p.ToString());

			p = new Path("a/''//b");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("'/", p.Segments[1].ResourceKey);
			Assert.AreEqual("b", p.Segments[2].ResourceKey);
			Assert.AreEqual("a/''//b", p.ToString());

			p = new Path("a/'/'/b");
			Assert.AreEqual(2, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("//b", p.Segments[1].ResourceKey);
			Assert.AreEqual("a/'/'/b", p.ToString());

			p = new Path("a/'/'//b");
			Assert.AreEqual(3, p.Segments.Count);
			Assert.AreEqual("a", p.Segments[0].ResourceKey);
			Assert.AreEqual("//", p.Segments[1].ResourceKey);
			Assert.AreEqual("b", p.Segments[2].ResourceKey);
			Assert.AreEqual("a/'/'//b", p.ToString());
		}

	}
}

#endif
