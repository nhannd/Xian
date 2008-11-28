#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class InequalitySpecificationTests : TestsBase
	{
		[Test]
		public void Test_GreaterThan_Exclusive()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsTrue(s.Test(2).Success);

			// null is less than any other value
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_GreaterThan_Inclusive()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			s.Inclusive = true;

			Assert.IsFalse(s.Test(0).Success);
			Assert.IsTrue(s.Test(1).Success);
			Assert.IsTrue(s.Test(2).Success);

			// null is less than any other value
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		// This test is currently failing because coercion code hasn't been merged to trunk yet
		public void Test_GreaterThan_CoerceTypes()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");

			Assert.IsFalse(s.Test(0).Success);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsTrue(s.Test(2).Success);
			Assert.IsTrue(s.Test(null).Success);
			Assert.IsTrue(s.Test(1).Success);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_GreaterThan_Strict()
		{
			GreaterThanSpecification s = new GreaterThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");
			//TODO: uncomment this when code merged to Trunk
			//s.Strict = true;

			// this should fail because in strict mode we don't do type coercion,
			// and IComparable throws an ArgumentException in this situation
			s.Test(0);
		}

		[Test]
		public void Test_LessThan_Exclusive()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			Assert.IsTrue(s.Test(0).Success);
			Assert.IsFalse(s.Test(1).Success);
			Assert.IsFalse(s.Test(2).Success);

			// null is less than any other value
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_LessThan_Inclusive()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression(1);
			s.Inclusive = true;

			Assert.IsTrue(s.Test(0).Success);
			Assert.IsTrue(s.Test(1).Success);
			Assert.IsFalse(s.Test(2).Success);

			// null is less than any other value
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		// This test is currently failing because coercion code hasn't been merged to trunk yet
		public void Test_LessThan_CoerceTypes()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");

			Assert.IsTrue(s.Test(0).Success);
			Assert.IsTrue(s.Test(1).Success);
			Assert.IsFalse(s.Test(2).Success);
			Assert.IsFalse(s.Test(null).Success);
			Assert.IsFalse(s.Test(1).Success);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Test_LessThan_Strict()
		{
			LessThanSpecification s = new LessThanSpecification();
			s.RefValueExpression = new ConstantExpression("1");
			//TODO: uncomment this when code merged to Trunk
			//s.Strict = true;

			// this should fail because in strict mode we don't do type coercion,
			// and IComparable throws an ArgumentException in this situation
			s.Test(0);
		}
	}
}

#endif
