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
			catch (EntityValidationException e)
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
			catch (EntityValidationException e)
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
			catch (EntityValidationException e)
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
