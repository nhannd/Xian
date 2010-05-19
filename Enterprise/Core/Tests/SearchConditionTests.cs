#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using NUnit.Framework;

namespace ClearCanvas.Enterprise.Core.Tests
{
	[TestFixture]
	public class SearchConditionTests
	{
		public SearchConditionTests()
		{
		}

		#region Cloning

		[Test]
		public void Test_CopyConstructor()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();

			sc.Between(x, y);
			sc.SortDesc(3);

			var copy = new SearchCondition<object>(sc);

			Assert.AreEqual(sc.Test, copy.Test);
			Assert.AreEqual(sc.Values.Length, copy.Values.Length);
			Assert.AreEqual(sc.Values[0], copy.Values[0]);
			Assert.AreEqual(sc.Values[1], copy.Values[1]);
			Assert.AreEqual(sc.SortDirection, copy.SortDirection);
			Assert.AreEqual(sc.SortPosition, copy.SortPosition);
			Assert.IsFalse(ReferenceEquals(sc.Values, copy.Values));
		}

		[Test]
		public void Test_Clone()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();

			sc.Between(x, y);
			sc.SortDesc(3);

			var copy = (SearchCondition<object>)sc.Clone();

			Assert.AreEqual(sc.Test, copy.Test);
			Assert.AreEqual(sc.Values.Length, copy.Values.Length);
			Assert.AreEqual(sc.Values[0], copy.Values[0]);
			Assert.AreEqual(sc.Values[1], copy.Values[1]);
			Assert.AreEqual(sc.SortDirection, copy.SortDirection);
			Assert.AreEqual(sc.SortPosition, copy.SortPosition);
			Assert.IsFalse(ReferenceEquals(sc.Values, copy.Values));
		}


		#endregion


		#region Setting Conditions

		[Test]
		public void Test_Empty()
		{
			var sc = new SearchCondition<object>();
			Assert.AreEqual(0, sc.Values.Length);
			Assert.AreEqual(SearchConditionTest.None, sc.Test);
			Assert.IsTrue(sc.IsEmpty);
		}

		[Test]
		public void Test_EqualTo()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.EqualTo(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.Equal, sc.Test);
		}

		[Test]
		public void Test_NotEqualTo()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.NotEqualTo(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.NotEqual, sc.Test);
		}

		[Test]
		public void Test_Like()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.Like(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.Like, sc.Test);
		}

		[Test]
		public void Test_NotLike()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.NotLike(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.NotLike, sc.Test);
		}

		[Test]
		public void Test_In()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();
			var z = new object();
			sc.In(new[]{x, y, z});

			Assert.AreEqual(3, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(y, sc.Values[1]);
			Assert.AreEqual(z, sc.Values[2]);
			Assert.AreEqual(SearchConditionTest.In, sc.Test);
		}

		[Test]
		public void Test_NotIn()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();
			var z = new object();
			sc.NotIn(new[] { x, y, z });

			Assert.AreEqual(3, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(y, sc.Values[1]);
			Assert.AreEqual(z, sc.Values[2]);
			Assert.AreEqual(SearchConditionTest.NotIn, sc.Test);
		}

		[Test]
		public void Test_IsNull()
		{
			var sc = new SearchCondition<object>();
			sc.IsNull();

			Assert.AreEqual(0, sc.Values.Length);
			Assert.AreEqual(SearchConditionTest.Null, sc.Test);
		}

		[Test]
		public void Test_IsNotNull()
		{
			var sc = new SearchCondition<object>();
			sc.IsNotNull();

			Assert.AreEqual(0, sc.Values.Length);
			Assert.AreEqual(SearchConditionTest.NotNull, sc.Test);
		}

		[Test]
		public void Test_Between()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();
			sc.Between(x, y);

			Assert.AreEqual(2, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(y, sc.Values[1]);
			Assert.AreEqual(SearchConditionTest.Between, sc.Test);
		}


		[Test]
		public void Test_LessThan()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.LessThan(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.LessThan, sc.Test);
		}

		[Test]
		public void Test_LessThanOrEqualTo()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.LessThanOrEqualTo(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.LessThanOrEqual, sc.Test);
		}

		[Test]
		public void Test_MoreThan()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.MoreThan(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.MoreThan, sc.Test);
		}

		[Test]
		public void Test_MoreThanOrEqualTo()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.MoreThanOrEqualTo(x);

			Assert.AreEqual(1, sc.Values.Length);
			Assert.AreEqual(x, sc.Values[0]);
			Assert.AreEqual(SearchConditionTest.MoreThanOrEqual, sc.Test);
		}

		#endregion

		#region Sorting

		[Test]
		public void Test_SortAsc()
		{
			var sc = new SearchCondition<object>();
			Assert.AreEqual(-1, sc.SortPosition);
			sc.SortAsc(0);

			Assert.AreEqual(true, sc.SortDirection);
			Assert.AreEqual(0, sc.SortPosition);

			sc.SortAsc(10);

			Assert.AreEqual(true, sc.SortDirection);
			Assert.AreEqual(10, sc.SortPosition);
		}

		[Test]
		public void Test_SortDesc()
		{
			var sc = new SearchCondition<object>();
			Assert.AreEqual(-1, sc.SortPosition);
			sc.SortDesc(0);

			Assert.AreEqual(false, sc.SortDirection);
			Assert.AreEqual(0, sc.SortPosition);

			sc.SortDesc(10);

			Assert.AreEqual(false, sc.SortDirection);
			Assert.AreEqual(10, sc.SortPosition);
		}

		#endregion

		#region IsSatisfiedBy tests

		[Test]
		public void Test_IsSatisfiedBy_EqualTo()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.EqualTo(x);

			Assert.IsTrue(sc.IsSatisfiedBy(x));
			Assert.IsFalse(sc.IsSatisfiedBy(new object()));
		}

		[Test]
		public void Test_IsSatisfiedBy_NotEqualTo()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			sc.NotEqualTo(x);

			Assert.IsFalse(sc.IsSatisfiedBy(x));
			Assert.IsTrue(sc.IsSatisfiedBy(new object()));
		}

		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void Test_IsSatisfiedBy_Like()
		{
			var sc = new SearchCondition<string>();
			var x = "x";
			sc.Like(x);

			// not implemented for Like
			sc.IsSatisfiedBy(x);
		}

		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void Test_IsSatisfiedBy_NotLike()
		{
			var sc = new SearchCondition<string>();
			var x = "x";
			sc.NotLike(x);

			// not implemented for NotLike
			sc.IsSatisfiedBy(x);
		}

		[Test]
		public void Test_IsSatisfiedBy_In()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();
			sc.In(new[]{x, y});

			Assert.IsTrue(sc.IsSatisfiedBy(x));
			Assert.IsTrue(sc.IsSatisfiedBy(y));
			Assert.IsFalse(sc.IsSatisfiedBy(new object()));
		}

		[Test]
		public void Test_IsSatisfiedBy_NotIn()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();
			sc.NotIn(new[] { x, y });

			Assert.IsFalse(sc.IsSatisfiedBy(x));
			Assert.IsFalse(sc.IsSatisfiedBy(y));
			Assert.IsTrue(sc.IsSatisfiedBy(new object()));
		}

		[Test]
		public void Test_IsSatisfiedBy_IsNull()
		{
			var sc = new SearchCondition<object>();
			sc.IsNull();

			Assert.IsTrue(sc.IsSatisfiedBy(null));
			Assert.IsFalse(sc.IsSatisfiedBy(new object()));
		}

		[Test]
		public void Test_IsSatisfiedBy_IsNotNull()
		{
			var sc = new SearchCondition<object>();
			sc.IsNotNull();

			Assert.IsFalse(sc.IsSatisfiedBy(null));
			Assert.IsTrue(sc.IsSatisfiedBy(new object()));
		}

		[Test]
		public void Test_IsSatisfiedBy_Between()
		{
			var sc = new SearchCondition<int>();
			sc.Between(1, 3);

			Assert.IsTrue(sc.IsSatisfiedBy(1));
			Assert.IsTrue(sc.IsSatisfiedBy(2));
			Assert.IsFalse(sc.IsSatisfiedBy(0));
			Assert.IsFalse(sc.IsSatisfiedBy(3));
		}

		[Test]
		public void Test_IsSatisfiedBy_LessThan()
		{
			var sc = new SearchCondition<int>();
			sc.LessThan(1);

			Assert.IsTrue(sc.IsSatisfiedBy(0));
			Assert.IsFalse(sc.IsSatisfiedBy(1));
			Assert.IsFalse(sc.IsSatisfiedBy(2));
		}

		[Test]
		public void Test_IsSatisfiedBy_LessThanOrEqualTo()
		{
			var sc = new SearchCondition<int>();
			sc.LessThanOrEqualTo(1);

			Assert.IsTrue(sc.IsSatisfiedBy(0));
			Assert.IsTrue(sc.IsSatisfiedBy(1));
			Assert.IsFalse(sc.IsSatisfiedBy(2));
		}

		[Test]
		public void Test_IsSatisfiedBy_MoreThan()
		{
			var sc = new SearchCondition<int>();
			sc.MoreThan(1);

			Assert.IsFalse(sc.IsSatisfiedBy(0));
			Assert.IsFalse(sc.IsSatisfiedBy(1));
			Assert.IsTrue(sc.IsSatisfiedBy(2));
		}

		[Test]
		public void Test_IsSatisfiedBy_MoreThanOrEqualTo()
		{
			var sc = new SearchCondition<int>();
			sc.MoreThanOrEqualTo(1);

			Assert.IsFalse(sc.IsSatisfiedBy(0));
			Assert.IsTrue(sc.IsSatisfiedBy(1));
			Assert.IsTrue(sc.IsSatisfiedBy(2));
		}

		#endregion

		#region IsSatisfiedBy Null tests

		[Test]
		public void Test_IsSatisfiedBy_EqualTo_Null()
		{
			var sc = new SearchCondition<object>();
			var x = new object();

			//expect same behaviour as SQL
			sc.EqualTo(x);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_NotEqualTo_Null()
		{
			var sc = new SearchCondition<object>();
			var x = new object();

			//expect same behaviour as SQL
			sc.NotEqualTo(x);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void Test_IsSatisfiedBy_Like_Null()
		{
			var sc = new SearchCondition<string>();
			var x = "x";
			sc.Like(x);

			// not implemented for Like
			sc.IsSatisfiedBy(x);
		}

		[Test]
		[ExpectedException(typeof(NotImplementedException))]
		public void Test_IsSatisfiedBy_NotLike_Null()
		{
			var sc = new SearchCondition<string>();
			var x = "x";
			sc.NotLike(x);

			// not implemented for Like
			sc.IsSatisfiedBy(x);
		}

		[Test]
		public void Test_IsSatisfiedBy_In_Null()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();

			//expect same behaviour as SQL
			sc.In(new[] { x, y });
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_NotIn_Null()
		{
			var sc = new SearchCondition<object>();
			var x = new object();
			var y = new object();

			//expect same behaviour as SQL
			sc.NotIn(new[] { x, y });
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_IsNull_Null()
		{
			var sc = new SearchCondition<object>();
			//expect same behaviour as SQL
			sc.IsNull();
			Assert.IsTrue(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_IsNotNull_Null()
		{
			var sc = new SearchCondition<object>();
			//expect same behaviour as SQL
			sc.IsNotNull();
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_Between_Null()
		{
			var sc = new SearchCondition<int>();
			//expect same behaviour as SQL
			sc.Between(1, 3);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_LessThan_Null()
		{
			var sc = new SearchCondition<int>();
			//expect same behaviour as SQL
			sc.LessThan(1);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_LessThanOrEqualTo_Null()
		{
			var sc = new SearchCondition<int>();
			//expect same behaviour as SQL
			sc.LessThanOrEqualTo(1);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_MoreThan_Null()
		{
			var sc = new SearchCondition<int>();
			//expect same behaviour as SQL
			sc.MoreThan(1);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		[Test]
		public void Test_IsSatisfiedBy_MoreThanOrEqualTo_Null()
		{
			var sc = new SearchCondition<int>();
			//expect same behaviour as SQL
			sc.MoreThanOrEqualTo(1);
			Assert.IsFalse(sc.IsSatisfiedBy(null));
		}

		#endregion
	}
}

#endif
