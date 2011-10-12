#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

#if UNIT_TESTS

using NUnit.Framework;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling.Tests
{
	[TestFixture]
	public class DomainObjectValidatorTests
	{
		class FooA : DomainObject
		{
			[Length(5)]
			public string Name { get; set; }
		}

		[Validation(EnableValidation = false)]
		class FooB : DomainObject
		{
			[Length(5)]
			public string Name { get; set; }

		}

		[Validation(EnableValidation = true)]
		class FooC : DomainObject
		{
			[Length(5)]
			public string Name { get; set; }
		}

		class FooE : FooB
		{
			[Required]
			public string Color { get; set; }
		}

		[Validation(EnableValidation = false)]
		class FooF : FooC
		{
			[Required]
			public string Color { get; set; }
		}

		[Validation(EnableValidation = true)]
		class FooG : FooB
		{
			[Required]
			public string Color { get; set; }
		}

		public DomainObjectValidatorTests()
		{
			Platform.SetExtensionFactory(new NullExtensionFactory());
		}

		[Test]
		public void Test_IsValidationEnabled_not_explicitly_specified()
		{
			// when no attribute is supplied on the class, the default is that validation is enabled
			Assert.IsTrue(DomainObjectValidator.IsValidationEnabled(typeof(FooA)));
		}

		[Test]
		public void Test_IsValidationEnabled_with_validation_disabled()
		{
			Assert.IsFalse(DomainObjectValidator.IsValidationEnabled(typeof(FooB)));
		}

		[Test]
		public void Test_IsValidationEnabled_with_validation_explicitly_enabled()
		{
			Assert.IsTrue(DomainObjectValidator.IsValidationEnabled(typeof(FooC)));
		}

		[Test]
		public void Test_IsValidationEnabled_with_inherited_attribute()
		{
			// confirm that FooE inherits behaviour of FooB
			Assert.AreEqual(
				DomainObjectValidator.IsValidationEnabled(typeof(FooB)),
				DomainObjectValidator.IsValidationEnabled(typeof(FooE))
				);
		}

		[Test]
		public void Test_IsValidationEnabled_with_overridden_attribute()
		{
			// confirm that FooF overrides what is inherited from FooC
			Assert.AreNotEqual(
				DomainObjectValidator.IsValidationEnabled(typeof(FooC)),
				DomainObjectValidator.IsValidationEnabled(typeof(FooF))
				);
		}

		[Test]
		public void Test_Validate()
		{
			try
			{
				var foo = new FooA() {Name = "Bethany"};
				var validator = new DomainObjectValidator();
				validator.Validate(foo);

				Assert.Fail("expected validation failure");
			}
			catch (EntityValidationException e)
			{
				// exactly one broken rule
				Assert.AreEqual(1, e.Reasons.Length);
			}
		}

		[Test]
		public void Test_Validate_validation_disabled()
		{
			try
			{
				var foo = new FooB() { Name = "Bethany" };
				var validator = new DomainObjectValidator();
				validator.Validate(foo);
			}
			catch (EntityValidationException)
			{
				Assert.Fail("validation was disabled and should not have failed");
			}
		}

		[Test]
		public void Test_Validate_validation_disabled_via_inherited_attribute()
		{
			try
			{
				var foo = new FooE() { Name = "Bethany" };
				var validator = new DomainObjectValidator();
				validator.Validate(foo);
			}
			catch (EntityValidationException)
			{
				Assert.Fail("validation was disabled and should not have failed");
			}
		}

		[Test]
		public void Test_Validate_validation_disabled_via_overriding_attribute()
		{
			try
			{
				var foo = new FooF() { Name = "Bethany" };
				var validator = new DomainObjectValidator();
				validator.Validate(foo);
			}
			catch (EntityValidationException)
			{
				Assert.Fail("validation was disabled and should not have failed");
			}
		}

		[Test]
		public void Test_Validate_validation_enabled_via_overriding_attribute()
		{
			try
			{
				var foo = new FooG() { Name = "Bethany" };
				var validator = new DomainObjectValidator();
				validator.Validate(foo);

				Assert.Fail("expected validation failure");
			}
			catch (EntityValidationException e)
			{
				// note:exactly 2 broken rules: this is important!!!
				// even though one of the rules was defined on a property of the base class,
				// and validation is disabled on the base class, the rule is still
				// evaluated for the subclass FooG which has validation enabled
				Assert.AreEqual(2, e.Reasons.Length);
			}
		}
	}
}

#endif
