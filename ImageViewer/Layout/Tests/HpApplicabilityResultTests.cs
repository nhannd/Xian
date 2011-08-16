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
	public class HpApplicabilityResultTests
	{
		HpApplicabilityResult result1 = HpApplicabilityResult.Positive;

		HpApplicabilityResult result3 = HpApplicabilityResult.Sum(new[] {HpApplicabilityResult.Positive, HpApplicabilityResult.Positive, HpApplicabilityResult.Positive});

		[Test]
		public void Test_identities()
		{
			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Negative);
			Assert.AreNotEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Neutral);
			Assert.AreNotEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Positive);

			Assert.AreEqual(HpApplicabilityResult.Neutral, HpApplicabilityResult.Neutral);
			Assert.AreNotEqual(HpApplicabilityResult.Neutral, HpApplicabilityResult.Positive);

			Assert.AreEqual(HpApplicabilityResult.Positive, HpApplicabilityResult.Positive);
		}

		[Test]
		public void Test_results_of_unequal_quality_are_not_equal()
		{
			// matches of unequal quality are not equal
			Assert.AreNotEqual(result1, result3);
		}

		[Test]
		public void Test_only_results_of_zero_quality_are_equal_to_Neutral()
		{
			Assert.AreNotEqual(HpApplicabilityResult.Neutral, result1);
			Assert.AreNotEqual(HpApplicabilityResult.Neutral, result3);
		}

		[Test]
		public void Test_addition_with_Negative()
		{
			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Negative + HpApplicabilityResult.Negative);
			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Negative + HpApplicabilityResult.Neutral);
			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Neutral + HpApplicabilityResult.Negative);
			Assert.AreEqual(HpApplicabilityResult.Negative, result1 + HpApplicabilityResult.Negative);
			Assert.AreEqual(HpApplicabilityResult.Negative, result3 + HpApplicabilityResult.Negative);
		}

		[Test]
		public void Test_addition_with_Neutral()
		{
			Assert.AreEqual(HpApplicabilityResult.Neutral, HpApplicabilityResult.Neutral + HpApplicabilityResult.Neutral);
			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Negative + HpApplicabilityResult.Neutral);
			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Neutral + HpApplicabilityResult.Negative);
			Assert.AreEqual(result1, result1 + HpApplicabilityResult.Neutral);
			Assert.AreEqual(result3, result3 + HpApplicabilityResult.Neutral);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Test_access_to_Quality_of_Negative_throws()
		{
			var q = HpApplicabilityResult.Negative.Quality;
		}

		[Test]
		public void Test_Sum()
		{
			var sum = HpApplicabilityResult.Sum(new[] { HpApplicabilityResult.Neutral, result1, result3 });
			Assert.IsTrue(sum.IsApplicable);
			Assert.AreEqual(4, sum.Quality);

			Assert.AreEqual(HpApplicabilityResult.Negative, HpApplicabilityResult.Sum(new[] { HpApplicabilityResult.Neutral, result1, result3, HpApplicabilityResult.Negative }));
		}
	}
}

#endif