#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class EnumerableSpecificationTests : TestsBase
	{
		[Test]
		public void Test_Each_Empty()
		{
			EachSpecification s1 = new EachSpecification(AlwaysFalse);
			Assert.IsTrue(s1.Test(new object[0]).Success);

			EachSpecification s2 = new EachSpecification(AlwaysTrue);
			Assert.IsTrue(s2.Test(new object[0]).Success);
		}

		[Test]
		public void Test_Each_Normal()
		{
			EachSpecification s = new EachSpecification(new PredicateSpecification<int>(delegate(int i) { return i > 0; }));
			Assert.IsFalse(s.Test(new int[] { 0, 1, 2 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 2, 3 }).Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_Each_InvalidType()
		{
			// cannot test a non-enumerable object
			EachSpecification s = new EachSpecification(AlwaysTrue);
			s.Test(new object());
		}

		[Test]
		public void Test_Any_Empty()
		{
			AnySpecification s1 = new AnySpecification(AlwaysFalse);
			Assert.IsFalse(s1.Test(new object[0]).Success);

			AnySpecification s2 = new AnySpecification(AlwaysTrue);
			Assert.IsFalse(s2.Test(new object[0]).Success);
		}

		[Test]
		public void Test_Any_Normal()
		{
			AnySpecification s = new AnySpecification(new PredicateSpecification<int>(delegate(int i) { return i > 0; }));
			Assert.IsFalse(s.Test(new int[] { 0, 0, 0 }).Success);
			Assert.IsTrue(s.Test(new int[] { 0, 0, 1 }).Success);
			Assert.IsTrue(s.Test(new int[] { 1, 1, 1 }).Success);
		}

		[Test]
		[ExpectedException(typeof(SpecificationException))]
		public void Test_Any_InvalidType()
		{
			// cannot test a non-enumerable object
			AnySpecification s = new AnySpecification(AlwaysTrue);
			s.Test(new object());
		}

	}
}

#endif
