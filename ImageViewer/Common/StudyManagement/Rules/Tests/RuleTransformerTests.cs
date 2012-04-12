#if UNIT_TESTS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using ClearCanvas.Common.Specifications;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules.Tests
{
	[TestFixture]
	class RuleTransformerTests
	{
		[Test]
		public void Test_no_conditions()
		{
			var rule = new RuleData();
			var transformer = new RuleTransformer();
			var xml = transformer.Conditions(rule.Conditions, rule.Junction);

			var spec = Compile(xml);
			Assert.IsInstanceOfType(typeof(NotNullSpecification), spec);

			var notNull = (NotNullSpecification)spec;
			Assert.AreEqual("$StudyInstanceUid", notNull.TestExpression.Text);
		}

		[Test]
		public void Test_match_all_conditions()
		{
			var rule = new RuleData {Junction = JunctionType.MatchAllConditions };
			rule.Conditions.Add(new RuleCondition {TagName = "Modality", Operator = ComparisonOperator.EqualTo, Value = "CT"});
			
			var transformer = new RuleTransformer();
			var xml = transformer.Conditions(rule.Conditions, rule.Junction);

			var spec = Compile(xml);
			Assert.IsInstanceOfType(typeof(AndSpecification), spec);

			var junction = (AndSpecification)spec;
			Assert.AreEqual(1, junction.Elements.Count());
		}

		[Test]
		public void Test_match_any_condition()
		{
			var rule = new RuleData { Junction = JunctionType.MatchAnyCondition };
			rule.Conditions.Add(new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.EqualTo, Value = "CT" });

			var transformer = new RuleTransformer();
			var xml = transformer.Conditions(rule.Conditions, rule.Junction);

			var spec = Compile(xml);
			Assert.IsInstanceOfType(typeof(OrSpecification), spec);

			var junction = (OrSpecification)spec;
			Assert.AreEqual(1, junction.Elements.Count());
		}

		[Test]
		public void Test_multiple_conditions()
		{
			var rule = new RuleData();
			rule.Conditions.Add(new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.EqualTo, Value = "CT" });
			rule.Conditions.Add(new RuleCondition { TagName = "StudyDescription", Operator = ComparisonOperator.EqualTo, Value = "Chest" });

			var transformer = new RuleTransformer();
			var xml = transformer.Conditions(rule.Conditions, rule.Junction);

			var spec = Compile(xml);
			Assert.IsInstanceOfType(typeof(AndSpecification), spec);

			var junction = (CompositeSpecification)spec;
			Assert.AreEqual(2, junction.Elements.Count());

			// try it again using "match any"
			rule.Junction = JunctionType.MatchAnyCondition;
			xml = transformer.Conditions(rule.Conditions, rule.Junction);

			spec = Compile(xml);
			Assert.IsInstanceOfType(typeof(OrSpecification), spec);

			junction = (CompositeSpecification)spec;
			Assert.AreEqual(2, junction.Elements.Count());
		}

		[Test]
		public void Test_condition_EqualTo()
		{
			var condition = new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.EqualTo, Value = "CT" };
			var transformer = new RuleTransformer();
			var xml = transformer.Condition(condition);

			var spec = Compile(new XElement(XName.Get("condition"), xml));
			Assert.IsInstanceOfType(typeof(RegexSpecification), spec);

			var typedSpec = (RegexSpecification)spec;
			Assert.AreEqual("$Modality", typedSpec.TestExpression.Text);
			Assert.AreEqual("^CT$", typedSpec.Pattern);
			Assert.IsTrue(typedSpec.IgnoreCase);
			Assert.IsFalse(typedSpec.NullMatches);
		}

		[Test]
		public void Test_condition_NotEqualTo()
		{
			var condition = new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.NotEqualTo, Value = "CT" };
			var transformer = new RuleTransformer();
			var xml = transformer.Condition(condition);

			var spec = Compile(new XElement(XName.Get("condition"), xml));

			Assert.IsInstanceOfType(typeof(NotSpecification), spec);
			var notSpec = (NotSpecification) spec;

			Assert.IsInstanceOfType(typeof(RegexSpecification), notSpec.Elements.First());

			var typedSpec = (RegexSpecification)notSpec.Elements.First();
			Assert.AreEqual("$Modality", typedSpec.TestExpression.Text);
			Assert.AreEqual("^CT$", typedSpec.Pattern);
			Assert.IsTrue(typedSpec.IgnoreCase);
			Assert.IsFalse(typedSpec.NullMatches);
		}

		[Test]
		public void Test_condition_Contains()
		{
			var condition = new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.Contains, Value = "CT" };
			var transformer = new RuleTransformer();
			var xml = transformer.Condition(condition);

			var spec = Compile(new XElement(XName.Get("condition"), xml));
			Assert.IsInstanceOfType(typeof(RegexSpecification), spec);

			var typedSpec = (RegexSpecification)spec;
			Assert.AreEqual("$Modality", typedSpec.TestExpression.Text);
			Assert.AreEqual("CT", typedSpec.Pattern);
			Assert.IsTrue(typedSpec.IgnoreCase);
			Assert.IsFalse(typedSpec.NullMatches);
		}

		[Test]
		public void Test_condition_DoesNotContain()
		{
			var condition = new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.DoesNotContain, Value = "CT" };
			var transformer = new RuleTransformer();
			var xml = transformer.Condition(condition);

			var spec = Compile(new XElement(XName.Get("condition"), xml));

			Assert.IsInstanceOfType(typeof(NotSpecification), spec);
			var notSpec = (NotSpecification)spec;

			Assert.IsInstanceOfType(typeof(RegexSpecification), notSpec.Elements.First());

			var typedSpec = (RegexSpecification)notSpec.Elements.First();
			Assert.AreEqual("$Modality", typedSpec.TestExpression.Text);
			Assert.AreEqual("CT", typedSpec.Pattern);
			Assert.IsTrue(typedSpec.IgnoreCase);
			Assert.IsFalse(typedSpec.NullMatches);
		}

		[Test]
		public void Test_condition_StartsWith()
		{
			var condition = new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.StartsWith, Value = "CT" };
			var transformer = new RuleTransformer();
			var xml = transformer.Condition(condition);

			var spec = Compile(new XElement(XName.Get("condition"), xml));
			Assert.IsInstanceOfType(typeof(RegexSpecification), spec);

			var typedSpec = (RegexSpecification)spec;
			Assert.AreEqual("$Modality", typedSpec.TestExpression.Text);
			Assert.AreEqual("^CT", typedSpec.Pattern);
			Assert.IsTrue(typedSpec.IgnoreCase);
			Assert.IsFalse(typedSpec.NullMatches);
		}

		[Test]
		public void Test_condition_EndsWith()
		{
			var condition = new RuleCondition { TagName = "Modality", Operator = ComparisonOperator.EndsWith, Value = "CT" };
			var transformer = new RuleTransformer();
			var xml = transformer.Condition(condition);

			var spec = Compile(new XElement(XName.Get("condition"), xml));
			Assert.IsInstanceOfType(typeof(RegexSpecification), spec);

			var typedSpec = (RegexSpecification)spec;
			Assert.AreEqual("$Modality", typedSpec.TestExpression.Text);
			Assert.AreEqual("CT$", typedSpec.Pattern);
			Assert.IsTrue(typedSpec.IgnoreCase);
			Assert.IsFalse(typedSpec.NullMatches);
		}

		private static ISpecification Compile(XElement xml)
		{
			var compiler = new XmlSpecificationCompiler("dicom");
			return compiler.Compile(GetXmlElement(xml));
		}

		private static XmlElement GetXmlElement(XElement element)
		{
			using (var xmlReader = element.CreateReader())
			{
				var xmlDoc = new XmlDocument();
				xmlDoc.Load(xmlReader);
				return xmlDoc.DocumentElement;
			}
		}
	}
}

#endif
