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
using NUnit.Framework;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Core;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Hibernate.Hql.Tests
{
    [TestFixture]
    public class HqlConditionTests
    {


		public HqlConditionTests()
        {
        }

        [Test]
        public void TestExpressionFactoryMethods()
        {
            AreEqual(new HqlCondition("x = ?", new object[] { 1 }), HqlCondition.EqualTo("x", 1));
			AreEqual(new HqlCondition("x <> ?", new object[] { 1 }), HqlCondition.NotEqualTo("x", 1));
			AreEqual(new HqlCondition("x like ?", new object[] { "foo" }), HqlCondition.Like("x", "foo"));
			AreEqual(new HqlCondition("x not like ?", new object[] { "foo" }), HqlCondition.NotLike("x", "foo"));
			AreEqual(new HqlCondition("x between ? and ?", new object[] { 1, 2 }), HqlCondition.Between("x", 1, 2));
			AreEqual(new HqlCondition("x in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.In("x", 1, 2, 3));
			AreEqual(new HqlCondition("x not in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.NotIn("x", 1, 2, 3));
			AreEqual(new HqlCondition("x < ?", new object[] { 1 }), HqlCondition.LessThan("x", 1));
			AreEqual(new HqlCondition("x <= ?", new object[] { 1 }), HqlCondition.LessThanOrEqual("x", 1));
			AreEqual(new HqlCondition("x > ?", new object[] { 1 }), HqlCondition.MoreThan("x", 1));
			AreEqual(new HqlCondition("x >= ?", new object[] { 1 }), HqlCondition.MoreThanOrEqual("x", 1));
			AreEqual(new HqlCondition("x is null", new object[] {}), HqlCondition.IsNull("x"));
			AreEqual(new HqlCondition("x is not null", new object[] {}), HqlCondition.IsNotNull("x"));
		}

		[Test]
		public void TestExpressionFromSearchCriteria()
		{
			AreEqual(new HqlCondition("x = ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.Equal, new object[] { 1 }));
			AreEqual(new HqlCondition("x <> ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.NotEqual, new object[] { 1 }));
			AreEqual(new HqlCondition("x like ?", new object[] { "foo" }), HqlCondition.GetCondition("x", SearchConditionTest.Like, new object[] { "foo" }));
			AreEqual(new HqlCondition("x not like ?", new object[] { "foo" }), HqlCondition.GetCondition("x", SearchConditionTest.NotLike, new object[] { "foo" }));
			AreEqual(new HqlCondition("x between ? and ?", new object[] { 1, 2 }), HqlCondition.GetCondition("x", SearchConditionTest.Between, new object[] { 1,2 }));
			AreEqual(new HqlCondition("x in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.GetCondition("x", SearchConditionTest.In, new object[] { 1,2,3 }));
			AreEqual(new HqlCondition("x < ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.LessThan, new object[] { 1 }));
			AreEqual(new HqlCondition("x <= ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.LessThanOrEqual, new object[] { 1 }));
			AreEqual(new HqlCondition("x > ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.MoreThan, new object[] { 1 }));
			AreEqual(new HqlCondition("x >= ?", new object[] { 1 }), HqlCondition.GetCondition("x", SearchConditionTest.MoreThanOrEqual, new object[] { 1 }));
			AreEqual(new HqlCondition("x is null", new object[] { }), HqlCondition.GetCondition("x", SearchConditionTest.Null, new object[] { 1 }));
			AreEqual(new HqlCondition("x is not null", new object[] { }), HqlCondition.GetCondition("x", SearchConditionTest.NotNull, new object[] { 1 }));
		}

		public void TestInList()
		{
			List<int> numbers = new List<int>();
			numbers.Add(1);
			numbers.Add(2);
			numbers.Add(3);

			AreEqual(new HqlCondition("x in (?,?,?)", new object[] { 1, 2, 3 }), HqlCondition.In("x", numbers));
		}

		private static void AreEqual(HqlCondition c1, HqlCondition c2)
		{
			Assert.AreEqual(c1.Hql, c2.Hql);
			Assert.IsTrue(CollectionUtils.Equal<object>(c1.Parameters, c2.Parameters, true));
		}
    }
}

#endif
