#if UNIT_TESTS

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
