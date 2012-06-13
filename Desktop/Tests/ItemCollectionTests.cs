#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS
#pragma warning disable 1591

using System;
using System.ComponentModel;
using NUnit.Framework;


namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class ItemCollectionTests
	{
		class Foo
		{
			public Foo(string name)
			{
				Name = name;
			}

			public string Name { get; set; }
		}

		[Test]
		public void Test_FindInsertionPoint_non_existant_item()
		{
			var items = new ItemCollection<Foo>();
			items.AddRange(new []{ new Foo("b"), new Foo("b"),  new Foo("d")});

			var comparison = new Comparison<Foo>((x, y) => string.CompareOrdinal(x.Name, y.Name));

			Assert.AreEqual(0, items.FindInsertionPoint(new Foo("a"), comparison));
			Assert.AreEqual(2, items.FindInsertionPoint(new Foo("c"), comparison));
			Assert.AreEqual(3, items.FindInsertionPoint(new Foo("e"), comparison));
		}

		[Test]
		public void Test_FindInsertionPoint_non_existing_item()
		{
			var items = new ItemCollection<Foo>();
			items.AddRange(new[] { new Foo("b"), new Foo("b"), new Foo("d") });

			var comparison = new Comparison<Foo>((x, y) => string.CompareOrdinal(x.Name, y.Name));

			var b = items.FindInsertionPoint(new Foo("b"), comparison);
			Assert.IsTrue(b == 0 || b == 1);

			var d = items.FindInsertionPoint(new Foo("d"), comparison);
			Assert.IsTrue(d == 2);
		}

		[Test]
		public void Test_FindInsertionPoint_empty_list()
		{
			var items = new ItemCollection<Foo>();

			var comparison = new Comparison<Foo>((x, y) => string.CompareOrdinal(x.Name, y.Name));

			// insertion point is always zero
			Assert.AreEqual(0, items.FindInsertionPoint(new Foo("a"), comparison));
			Assert.AreEqual(0, items.FindInsertionPoint(new Foo("d"), comparison));
		}
	}
}

#endif