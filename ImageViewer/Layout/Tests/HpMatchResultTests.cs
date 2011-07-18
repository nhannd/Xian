#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Layout.Tests
{
	[TestFixture]
	public class HpMatchResultTests
	{
		HpMatchResult match1 = HpMatchResult.Positive;

		HpMatchResult match3 = HpMatchResult.Sum(new[] {HpMatchResult.Positive, HpMatchResult.Positive, HpMatchResult.Positive});

		[Test]
		public void Test_identities()
		{
			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Negative);
			Assert.AreNotEqual(HpMatchResult.Negative, HpMatchResult.Zero);
			Assert.AreNotEqual(HpMatchResult.Negative, HpMatchResult.Positive);

			Assert.AreEqual(HpMatchResult.Zero, HpMatchResult.Zero);
			Assert.AreNotEqual(HpMatchResult.Zero, HpMatchResult.Positive);

			Assert.AreEqual(HpMatchResult.Positive, HpMatchResult.Positive);
		}

		[Test]
		public void Test_matches_of_unequal_quality_are_not_equal()
		{
			// matches of unequal quality are not equal
			Assert.AreNotEqual(match1, match3);
		}

		[Test]
		public void Test_only_matches_of_zero_quality_are_equal_to_zero()
		{
			Assert.AreNotEqual(HpMatchResult.Zero, match1);
			Assert.AreNotEqual(HpMatchResult.Zero, match3);
		}

		[Test]
		public void Test_addition_with_Negative()
		{
			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Negative + HpMatchResult.Negative);
			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Negative + HpMatchResult.Zero);
			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Zero + HpMatchResult.Negative);
			Assert.AreEqual(HpMatchResult.Negative, match1 + HpMatchResult.Negative);
			Assert.AreEqual(HpMatchResult.Negative, match3 + HpMatchResult.Negative);
		}

		[Test]
		public void Test_addition_with_Zero()
		{
			Assert.AreEqual(HpMatchResult.Zero, HpMatchResult.Zero + HpMatchResult.Zero);
			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Negative + HpMatchResult.Zero);
			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Zero + HpMatchResult.Negative);
			Assert.AreEqual(match1, match1 + HpMatchResult.Zero);
			Assert.AreEqual(match3, match3 + HpMatchResult.Zero);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_access_to_Quality_of_Negative_throws()
		{
			var q = HpMatchResult.Negative.Quality;
		}

		[Test]
		public void Test_Sum()
		{
			var sum = HpMatchResult.Sum(new[] { HpMatchResult.Zero, match1, match3 });
			Assert.IsTrue(sum.IsMatch);
			Assert.AreEqual(4, sum.Quality);

			Assert.AreEqual(HpMatchResult.Negative, HpMatchResult.Sum(new[] { HpMatchResult.Zero, match1, match3, HpMatchResult.Negative }));
		}
	}
}

#endif