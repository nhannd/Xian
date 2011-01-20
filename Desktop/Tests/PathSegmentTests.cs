#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
	public class PathSegmentTests
	{
		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void Test_Constructor_Null()
		{
			new PathSegment(null);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_Constructor_Empty()
		{
			new PathSegment("");
		}

		[Test]
		public void Test_Constructor_NullLocalized()
		{
			var p = new PathSegment("a", (string)null);
			Assert.AreEqual("a", p.ResourceKey);
			Assert.AreEqual(null, p.LocalizedText);
		}

		[Test]
		public void Test_Constructor_EmptyLocalized()
		{
			var p = new PathSegment("a", "");
			Assert.AreEqual("a", p.ResourceKey);
			Assert.AreEqual("", p.LocalizedText);
		}

		[Test]
		public void Test_NullResolver()
		{
			var p = new PathSegment("a", (IResourceResolver) null);
			Assert.AreEqual("a", p.ResourceKey);
			Assert.AreEqual("a", p.LocalizedText);
		}
	}
}

#endif
