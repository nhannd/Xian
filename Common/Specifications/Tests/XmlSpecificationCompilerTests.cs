#if UNIT_TESTS

#pragma warning disable 1591

using System;
using System.IO;
using NUnit.Framework;
using System.Collections.Generic;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class XmlSpecificationCompilerTests : TestsBase
	{
		/// <summary>
		/// The XML Spec Compiler relies on certain extension points, therefore we need to stub out the extension
		/// factory so we can supply the dependencies.
		/// </summary>
		class StubExtensionFactory : IExtensionFactory
		{
			public object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne)
			{
				Console.WriteLine(extensionPoint);
				if (extensionPoint.GetType() == typeof(ExpressionFactoryExtensionPoint))
					return new object[] { new ConstantExpressionFactory() };
				if (extensionPoint.GetType() == typeof(XmlSpecificationCompilerOperatorExtensionPoint))
					return new object[]{};

				throw new NotSupportedException();
			}

			public ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter)
			{
				Console.WriteLine(extensionPoint);
				if (extensionPoint.GetType() == typeof(ExpressionFactoryExtensionPoint))
					return new ExtensionInfo[] { new ExtensionInfo(typeof(ConstantExpressionFactory), extensionPoint.GetType(), null, null),  };
				if (extensionPoint.GetType() == typeof(XmlSpecificationCompilerOperatorExtensionPoint))
					return new ExtensionInfo[] { };

				throw new NotSupportedException();
			}
		}


		private readonly SpecificationFactory _factory;

		public XmlSpecificationCompilerTests()
		{
			// stub the extension factory
			Platform.SetExtensionFactory(new StubExtensionFactory());

			// load the test file
			using (Stream s = this.GetType().Assembly.GetManifestResourceStream("ClearCanvas.Common.Specifications.Tests.XmlSpecificationCompilerTests.xml"))
			{
				_factory = new SpecificationFactory(s);
			}
		}

		[Test]
		public void Test_TestExpression()
		{
			ISpecification s = _factory.GetSpecification("testExpression");

			// the above evaluates to an implicit and
			CompositeSpecification s1 = (CompositeSpecification)s;
			foreach (Specification element in s1.Elements)
			{
				Assert.AreEqual("XXX", element.TestExpression.Text);
			}
		}

		[Test]
		public void Test_FailMessage()
		{
			ISpecification s = _factory.GetSpecification("failMessage");

			// the above evaluates to an implicit and
			CompositeSpecification s1 = (CompositeSpecification)s;
			foreach (Specification element in s1.Elements)
			{
				Assert.AreEqual("XXX", element.FailureMessage);
			}
		}

		[Test]
		public void Test_True_Default()
		{
			ISpecification s = _factory.GetSpecification("true_default");
			Assert.IsInstanceOfType(typeof(TrueSpecification), s);
		}

		[Test]
		public void Test_False_Default()
		{
			ISpecification s = _factory.GetSpecification("false_default");
			Assert.IsInstanceOfType(typeof(FalseSpecification), s);
		}

		[Test]
		public void Test_Null_Default()
		{
			ISpecification s = _factory.GetSpecification("null_default");
			Assert.IsInstanceOfType(typeof(IsNullSpecification), s);
		}

		[Test]
		public void Test_NotNull_Default()
		{
			ISpecification s = _factory.GetSpecification("notNull_default");
			Assert.IsInstanceOfType(typeof(NotNullSpecification), s);
		}

		[Test]
		public void Test_Regex_Default()
		{
			ISpecification s = _factory.GetSpecification("regex_default");
			Assert.IsInstanceOfType(typeof(RegexSpecification), s);

			RegexSpecification s1 = (RegexSpecification) s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		public void Test_Regex_Options1()
		{
			ISpecification s = _factory.GetSpecification("regex_options1");
			Assert.IsInstanceOfType(typeof(RegexSpecification), s);

			RegexSpecification s1 = (RegexSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(false, s1.NullMatches);
			Assert.AreEqual(false, s1.IgnoreCase);
		}

		[Test]
		public void Test_Regex_Options2()
		{
			ISpecification s = _factory.GetSpecification("regex_options2");
			Assert.IsInstanceOfType(typeof(RegexSpecification), s);

			RegexSpecification s1 = (RegexSpecification)s;
			Assert.AreEqual("XXX", s1.Pattern);
			Assert.AreEqual(true, s1.NullMatches);
			Assert.AreEqual(true, s1.IgnoreCase);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_Regex_MissingPattern()
		{
			ISpecification s = _factory.GetSpecification("regex_missingPattern");
		}

		[Test]
		public void Test_Equal_Default()
		{
			ISpecification s = _factory.GetSpecification("equal_default");
			Assert.IsInstanceOfType(typeof(EqualSpecification), s);

			EqualSpecification s1 = (EqualSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_Equal_MissingRefValue()
		{
			ISpecification s = _factory.GetSpecification("equal_missingRefValue");
		}

		[Test]
		public void Test_GreaterThan_Default()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_default");
			Assert.IsInstanceOfType(typeof(GreaterThanSpecification), s);

			GreaterThanSpecification s1 = (GreaterThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_GreaterThan_Options1()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_options1");
			Assert.IsInstanceOfType(typeof(GreaterThanSpecification), s);

			GreaterThanSpecification s1 = (GreaterThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_GreaterThan_Options2()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_options2");
			Assert.IsInstanceOfType(typeof(GreaterThanSpecification), s);

			GreaterThanSpecification s1 = (GreaterThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(true, s1.Inclusive);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_GreaterThan_MissingRefValue()
		{
			ISpecification s = _factory.GetSpecification("greaterThan_missingRefValue");
		}

		[Test]
		public void Test_LessThan_Default()
		{
			ISpecification s = _factory.GetSpecification("lessThan_default");
			Assert.IsInstanceOfType(typeof(LessThanSpecification), s);

			LessThanSpecification s1 = (LessThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_LessThan_Options1()
		{
			ISpecification s = _factory.GetSpecification("lessThan_options1");
			Assert.IsInstanceOfType(typeof(LessThanSpecification), s);

			LessThanSpecification s1 = (LessThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(false, s1.Inclusive);
		}

		[Test]
		public void Test_LessThan_Options2()
		{
			ISpecification s = _factory.GetSpecification("lessThan_options2");
			Assert.IsInstanceOfType(typeof(LessThanSpecification), s);

			LessThanSpecification s1 = (LessThanSpecification)s;
			Assert.AreEqual("XXX", s1.RefValueExpression.Text);
			Assert.AreEqual(true, s1.Inclusive);
		}

		[Test]
		[ExpectedException(typeof(XmlSpecificationCompilerException))]
		public void Test_LessThan_MissingRefValue()
		{
			ISpecification s = _factory.GetSpecification("lessThan_missingRefValue");
		}

		[Test]
		public void Test_Count_Default()
		{
			ISpecification s = _factory.GetSpecification("count_default");
			Assert.IsInstanceOfType(typeof(CountSpecification), s);

			CountSpecification s1 = (CountSpecification)s;
			Assert.AreEqual(0, s1.Min);
			Assert.AreEqual(int.MaxValue, s1.Max);
		}

		[Test]
		public void Test_Count_Options1()
		{
			ISpecification s = _factory.GetSpecification("count_options1");
			Assert.IsInstanceOfType(typeof(CountSpecification), s);

			CountSpecification s1 = (CountSpecification)s;
			Assert.AreEqual(1, s1.Min);
			Assert.AreEqual(int.MaxValue, s1.Max);
		}

		[Test]
		public void Test_Count_Options2()
		{
			ISpecification s = _factory.GetSpecification("count_options2");
			Assert.IsInstanceOfType(typeof(CountSpecification), s);

			CountSpecification s1 = (CountSpecification)s;
			Assert.AreEqual(0, s1.Min);
			Assert.AreEqual(2, s1.Max);
		}

		[Test]
		public void Test_Count_Options3()
		{
			ISpecification s = _factory.GetSpecification("count_options3");
			Assert.IsInstanceOfType(typeof(CountSpecification), s);

			CountSpecification s1 = (CountSpecification)s;
			Assert.AreEqual(1, s1.Min);
			Assert.AreEqual(2, s1.Max);
		}

		[Test]
		public void Test_Count_Filtered()
		{
			ISpecification s = _factory.GetSpecification("count_filtered");
			Assert.IsInstanceOfType(typeof(CountSpecification), s);

			CountSpecification s1 = (CountSpecification)s;
			Assert.AreEqual(1, s1.Min);
			Assert.AreEqual(2, s1.Max);
			Assert.IsNotNull(s1.FilterSpecification);
			Assert.IsInstanceOfType(typeof(TrueSpecification), s1.FilterSpecification);
		}

		[Test]
		public void Test_And_Default()
		{
			ISpecification s = _factory.GetSpecification("and_default");
			Assert.IsInstanceOfType(typeof(AndSpecification), s);

			AndSpecification s1 = (AndSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
			Assert.IsInstanceOfType(typeof(TrueSpecification), elements[0]);
			Assert.IsInstanceOfType(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_And_Implicit()
		{
			ISpecification s = _factory.GetSpecification("and_implicit");
			Assert.IsInstanceOfType(typeof(AndSpecification), s);

			AndSpecification s1 = (AndSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
			Assert.IsInstanceOfType(typeof(TrueSpecification), elements[0]);
			Assert.IsInstanceOfType(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_And_Empty()
		{
			ISpecification s = _factory.GetSpecification("and_empty");
			Assert.IsInstanceOfType(typeof(AndSpecification), s);

			AndSpecification s1 = (AndSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(0, elements.Count);
		}

		[Test]
		public void Test_Or_Default()
		{
			ISpecification s = _factory.GetSpecification("or_default");
			Assert.IsInstanceOfType(typeof(OrSpecification), s);

			OrSpecification s1 = (OrSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
			Assert.IsInstanceOfType(typeof(TrueSpecification), elements[0]);
			Assert.IsInstanceOfType(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_Or_Empty()
		{
			ISpecification s = _factory.GetSpecification("or_empty");
			Assert.IsInstanceOfType(typeof(OrSpecification), s);

			OrSpecification s1 = (OrSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(0, elements.Count);
		}

		[Test]
		public void Test_Not_Default()
		{
			ISpecification s = _factory.GetSpecification("not_default");
			Assert.IsInstanceOfType(typeof(NotSpecification), s);

			NotSpecification s1 = (NotSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(2, elements.Count);
			Assert.IsInstanceOfType(typeof(TrueSpecification), elements[0]);
			Assert.IsInstanceOfType(typeof(FalseSpecification), elements[1]);
		}

		[Test]
		public void Test_Not_Empty()
		{
			ISpecification s = _factory.GetSpecification("not_missingElement");
			Assert.IsInstanceOfType(typeof(NotSpecification), s);

			NotSpecification s1 = (NotSpecification)s;
			List<ISpecification> elements = new List<ISpecification>(s1.Elements);
			Assert.AreEqual(0, elements.Count);
		}

		[Test]
		public void Test_Each_Default()
		{
			ISpecification s = _factory.GetSpecification("each_default");
			Assert.IsInstanceOfType(typeof(EachSpecification), s);

			EachSpecification s1 = (EachSpecification)s;
			Assert.IsNotNull(s1.ElementSpec);
			Assert.IsInstanceOfType(typeof(TrueSpecification), s1.ElementSpec);
		}

		[Test]
		public void Test_Each_MissingElement()
		{
			//TODO: this scenario does not currently throw an exception - should it?
			ISpecification s = _factory.GetSpecification("each_missingElement");
			Assert.IsInstanceOfType(typeof(EachSpecification), s);
		}

		[Test]
		public void Test_Any_Default()
		{
			ISpecification s = _factory.GetSpecification("any_default");
			Assert.IsInstanceOfType(typeof(AnySpecification), s);

			AnySpecification s1 = (AnySpecification)s;
			Assert.IsNotNull(s1.ElementSpec);
			Assert.IsInstanceOfType(typeof(TrueSpecification), s1.ElementSpec);
		}

		[Test]
		public void Test_Any_MissingElement()
		{
			//TODO: this scenario does not currently throw an exception - should it?
			ISpecification s = _factory.GetSpecification("any_missingElement");
			Assert.IsInstanceOfType(typeof(AnySpecification), s);
		}
	}
}

#endif
