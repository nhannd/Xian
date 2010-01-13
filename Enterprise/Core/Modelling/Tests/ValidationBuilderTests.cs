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

using NUnit.Framework;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling.Tests
{
	[TestFixture]
	public class ValidationBuilderTests
	{
		class FooA
		{
			[Length(5)]
			public string Name { get; set; }
		}

		class FooB
		{
			[Required]
			public string Name { get; set; }
		}

		class FooC
		{
			[EmbeddedValue]
			public FooA EmbeddedFoo { get; set; }
		}

		class FooD
		{
			[Length(5)]
			[Required]
			public string Name { get; set; }
		}

		class FooE
		{
			[Length(5)]
			public string Name { get; set; }

			[Required]
			public string Color { get; set; }
		}

		[Test]
		public void Test_LengthSpecification()
		{
			var builder = new ValidationBuilder();
			var ruleSet = builder.BuildRuleSet(typeof(FooA));

			Assert.AreEqual(1, ruleSet.Rules.Count);
			object rule = ruleSet.Rules[0];
			Assert.IsInstanceOfType(typeof(LengthSpecification), rule);
			var property = CollectionUtils.FirstElement((rule as IPropertyBoundRule).Properties);
			Assert.AreEqual(typeof(FooA).GetProperty("Name"), property);

			var fooA = new FooA {Name = "Bob"};

			Assert.IsTrue(ruleSet.Test(fooA).Success);

			fooA.Name = "Robert";

			Assert.IsFalse(ruleSet.Test(fooA).Success);
		}

		[Test]
		public void Test_RequiredSpecification()
		{
			var builder = new ValidationBuilder();
			var ruleSet = builder.BuildRuleSet(typeof(FooB));

			Assert.AreEqual(1, ruleSet.Rules.Count);
			object rule = ruleSet.Rules[0];
			Assert.IsInstanceOfType(typeof(RequiredSpecification), rule);
			var property = CollectionUtils.FirstElement((rule as IPropertyBoundRule).Properties);
			Assert.AreEqual(typeof(FooB).GetProperty("Name"), property);

			var foo = new FooB {Name = "Bob"};

			Assert.IsTrue(ruleSet.Test(foo).Success);

			// should fail
			foo.Name = null;
			Assert.IsFalse(ruleSet.Test(foo).Success);

			// emtpy string should fail as well
			foo.Name = "";
			Assert.IsFalse(ruleSet.Test(foo).Success);
		}

		[Test]
		public void Test_EmbeddedValue()
		{
			var builder = new ValidationBuilder();
			var ruleSet = builder.BuildRuleSet(typeof(FooC));

			Assert.AreEqual(1, ruleSet.Rules.Count);
			object rule = ruleSet.Rules[0];
			var property = CollectionUtils.FirstElement((rule as IPropertyBoundRule).Properties);
			Assert.AreEqual(typeof(FooC).GetProperty("EmbeddedFoo"), property);

			var foo = new FooC {EmbeddedFoo = null};

			// this should pass, because rules on the embedded value class are not evaluated when the property is null
			Assert.IsTrue(ruleSet.Test(foo).Success);

			// should pass
			foo.EmbeddedFoo = new FooA {Name = "Bob"};
			Assert.IsTrue(ruleSet.Test(foo).Success);

			// should fail
			foo.EmbeddedFoo.Name = "Robert";
			Assert.IsFalse(ruleSet.Test(foo).Success);
		}

		[Test]
		public void Test_CombinationOfRequiredAndLengthOnSameProperty()
		{
			var builder = new ValidationBuilder();
			var ruleSet = builder.BuildRuleSet(typeof(FooD));

			var foo = new FooD();

			// should fail the "required" but not the "length" test
			var result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// should fail the "length" test but not the "required" test
			foo.Name = "Robert";
			result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// should fail neither
			foo.Name = "Bob";
			result = ruleSet.Test(foo);
			Assert.IsTrue(result.Success);
			Assert.AreEqual(0, result.Reasons.Length);
		}

		public void Test_MultipleBrokenRules()
		{
			var builder = new ValidationBuilder();
			var ruleSet = builder.BuildRuleSet(typeof(FooE));

			var foo = new FooE {Name = "Robert"};

			// should fail both
			var result = ruleSet.Test(foo);
			Assert.IsFalse(result.Success);
			Assert.AreEqual(2, result.Reasons.Length);

			// should fail only Name length
			foo.Color = "Blue";
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);

			// should fail neither
			foo.Name = "Bot";
			Assert.IsTrue(result.Success);
			Assert.AreEqual(0, result.Reasons.Length);

			// should fail Name required
			foo.Name = null;
			Assert.IsFalse(result.Success);
			Assert.AreEqual(1, result.Reasons.Length);
		}

	}
}

#endif
