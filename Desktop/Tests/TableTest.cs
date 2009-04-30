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
#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace ClearCanvas.Desktop.Tests
{
	[TestFixture]
	public class TableTest
	{
		[Test]
		public void TestSortingItemCollection()
		{
			new ItemCollection<Item>().Sort(new ComparerA()); // sorting an empty collection should not fail

			ItemCollection<Item> coll1 = new ItemCollection<Item>();
			coll1.Add(new Item(1, 2, 3));
			coll1.Sort(new ComparerA());
			AssertEquals("(1,2,3)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Add(new Item(3, 1, 1));
			coll1.Add(new Item(1, 8, 3));
			coll1.Add(new Item(1, 3, 3));
			coll1.Add(new Item(2, 4, 2));
			coll1.Add(new Item(2, 5, 2));
			coll1.Add(new Item(3, 6, 1));
			coll1.Add(new Item(3, 7, 1));
			coll1.Sort(new ComparerC());
			coll1.Sort(new ComparerB());
			coll1.Sort(new ComparerA());
			AssertEquals("(1,2,3),(1,3,3),(1,8,3),(2,4,2),(2,5,2),(3,1,1),(3,6,1),(3,7,1)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Sort(new ComparerC());
			coll1.Sort(new RevComparerB());
			coll1.Sort(new RevComparerA());
			AssertEquals("(3,7,1),(3,6,1),(3,1,1),(2,5,2),(2,4,2),(1,8,3),(1,3,3),(1,2,3)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Sort(new ComparerC());
			coll1.Sort(new ComparerB());
			coll1.Sort(new RevComparerA());
			AssertEquals("(3,1,1),(3,6,1),(3,7,1),(2,4,2),(2,5,2),(1,2,3),(1,3,3),(1,8,3)", coll1);

			// reset
			coll1.Sort(new Shuffle());

			coll1.Sort(new RevComparerC());
			coll1.Sort(new ComparerB());
			coll1.Sort(new ComparerA());
			AssertEquals("(1,2,3),(1,3,3),(1,8,3),(2,4,2),(2,5,2),(3,1,1),(3,6,1),(3,7,1)", coll1);
		}

		private static void AssertEquals(string expected, IEnumerable<Item> actual)
		{
			StringBuilder sb = new StringBuilder();
			foreach (Item item in actual)
			{
				sb.Append(item.ToString() + ",");
			}

			if (sb.Length == 0)
			{
				Assert.AreEqual(expected, sb.ToString());
			}
			else
			{
				Assert.AreEqual(expected, sb.ToString(0, sb.Length - 1));
			}
		}

		private class RevComparerA : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return -x.A.CompareTo(y.A);
			}
		}

		private class ComparerA : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return x.A.CompareTo(y.A);
			}
		}

		private class RevComparerB : IComparer<Item> {
			public int Compare(Item x, Item y) {
				return -x.B.CompareTo(y.B);
			}
		}

		private class ComparerB : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return x.B.CompareTo(y.B);
			}
		}

		private class RevComparerC : IComparer<Item> {
			public int Compare(Item x, Item y) {
				return -x.C.CompareTo(y.C);
			}
		}

		private class ComparerC : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return x.C.CompareTo(y.C);
			}
		}

		private class Shuffle : IComparer<Item>
		{
			public int Compare(Item x, Item y)
			{
				return new Random().Next(-1, 2);
			}
		}

		private struct Item
		{
			public int A;
			public int B;
			public int C;

			public Item(int a, int b, int c)
			{
				A = a;
				B = b;
				C = c;
			}

			public override string ToString()
			{
				return string.Format("({0},{1},{2})", A, B, C);
			}
		}
	}
}

#endif