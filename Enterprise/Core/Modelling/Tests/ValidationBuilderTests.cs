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

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;
using System.Reflection;

namespace ClearCanvas.Enterprise.Core.Modelling.Tests
{
    [TestFixture]
    public class ValidationBuilderTests
    {
        class FooA
        {
            private string _name;

            [Length(5)]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
        }

        class FooB
        {
            private string _name;

            [Required]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
        }

        class FooC
        {
            private FooA _embeddedFoo;

            [EmbeddedValue]
            public FooA EmbeddedFoo
            {
                get { return _embeddedFoo; }
                set { _embeddedFoo = value; }
            }
        }

        class FooD
        {
            private string _name;

            [Length(5)]
            [Required]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }
        }

        class FooE
        {
            private string _name;

            [Length(5)]
            public string Name
            {
                get { return _name; }
                set { _name = value; }
            }

            private string _color;

            [Required]
            public string Color
            {
                get { return _color; }
                set { _color = value; }
            }
        }


        public ValidationBuilderTests()
        {

        }

        [Test]
        public void Test_LengthSpecification()
        {
            ValidationBuilder builder = new ValidationBuilder();
            ValidationRuleSet ruleSet = builder.BuildRuleSet(typeof(FooA));

            Assert.AreEqual(1, ruleSet.Rules.Count);
            object rule = ruleSet.Rules[0];
            Assert.IsInstanceOfType(typeof(LengthSpecification), rule);
            PropertyInfo property = CollectionUtils.FirstElement<PropertyInfo>((rule as IPropertyBoundRule).Properties);
            Assert.AreEqual(typeof(FooA).GetProperty("Name"), property);

            FooA fooA = new FooA();
            fooA.Name = "Bob";

            Assert.IsTrue(ruleSet.Test(fooA).Success);

            fooA.Name = "Robert";

            Assert.IsFalse(ruleSet.Test(fooA).Success);
        }

        [Test]
        public void Test_RequiredSpecification()
        {
            ValidationBuilder builder = new ValidationBuilder();
            ValidationRuleSet ruleSet = builder.BuildRuleSet(typeof(FooB));

            Assert.AreEqual(1, ruleSet.Rules.Count);
            object rule = ruleSet.Rules[0];
            Assert.IsInstanceOfType(typeof(RequiredSpecification), rule);
            PropertyInfo property = CollectionUtils.FirstElement<PropertyInfo>((rule as IPropertyBoundRule).Properties);
            Assert.AreEqual(typeof(FooB).GetProperty("Name"), property);

            FooB foo = new FooB();
            foo.Name = "Bob";

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
            ValidationBuilder builder = new ValidationBuilder();
            ValidationRuleSet ruleSet = builder.BuildRuleSet(typeof(FooC));

            Assert.AreEqual(1, ruleSet.Rules.Count);
            object rule = ruleSet.Rules[0];
            PropertyInfo property = CollectionUtils.FirstElement<PropertyInfo>((rule as IPropertyBoundRule).Properties);
            Assert.AreEqual(typeof(FooC).GetProperty("EmbeddedFoo"), property);

            FooC foo = new FooC();

            // this should pass, because rules on the embedded value class are not evaluated when the property is null
            foo.EmbeddedFoo = null;
            Assert.IsTrue(ruleSet.Test(foo).Success);

            // should pass
            foo.EmbeddedFoo = new FooA();
            foo.EmbeddedFoo.Name = "Bob";
            Assert.IsTrue(ruleSet.Test(foo).Success);

            // should fail
            foo.EmbeddedFoo.Name = "Robert";
            Assert.IsFalse(ruleSet.Test(foo).Success);
        }

        [Test]
        public void Test_CombinationOfRequiredAndLengthOnSameProperty()
        {
            ValidationBuilder builder = new ValidationBuilder();
            ValidationRuleSet ruleSet = builder.BuildRuleSet(typeof(FooD));

            FooD foo = new FooD();

            // should fail the "required" but not the "length" test
            TestResult result = ruleSet.Test(foo);
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
            ValidationBuilder builder = new ValidationBuilder();
            ValidationRuleSet ruleSet = builder.BuildRuleSet(typeof(FooE));

            FooE foo = new FooE();
            foo.Name = "Robert";

            // should fail both
            TestResult result = ruleSet.Test(foo);
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
