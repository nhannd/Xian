using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Common.StudyManagement.Rules
{
	internal class RuleTransformer
	{
#if UNIT_TESTS
#endif


		public XElement Rule(RuleData input)
		{
			return new XElement(XName.Get("rule"), Conditions(input.Conditions, input.Junction));
		}

		public XElement Conditions(IList<RuleCondition> conditions, JunctionType junction)
		{
			var node = new XElement(XName.Get("condition"));
			node.SetAttributeValue(XName.Get("expressionLanguage"), "dicom");
			if (conditions.Count > 0)
			{
				node.Add(
					Junction(junction == JunctionType.MatchAllConditions ? "and" : "or", conditions.Select(Condition)));
			}
			else
			{
				// TODO: this is a dummy expr which should return true for all studies - is there a better way to do this??
				node.Add(Element("not-null", DicomTagDictionary.GetDicomTag("StudyInstanceUid"), new Dictionary<string, string>()));
			}

			return node;
		}

		public XElement Condition(RuleCondition input)
		{
			switch(input.Operator)
			{
				case ComparisonOperator.EqualTo:
					return Regex(input.Tag, string.Format("^{0}$", input.Value));
				case ComparisonOperator.NotEqualTo:
					return Not(Regex(input.Tag, string.Format("^{0}$", input.Value)));
				case ComparisonOperator.Contains:
					return Regex(input.Tag, string.Format("{0}", input.Value));
				case ComparisonOperator.DoesNotContain:
					return Not(Regex(input.Tag, string.Format("{0}", input.Value)));
				case ComparisonOperator.StartsWith:
					return Regex(input.Tag, string.Format("^{0}", input.Value));
				case ComparisonOperator.EndsWith:
					return Regex(input.Tag, string.Format("{0}$", input.Value));
			}
			throw new ArgumentOutOfRangeException();
		}

		private static XElement Junction(string op, IEnumerable<XElement> childNodes)
		{
			var node = new XElement(XName.Get(op));
			node.Add(childNodes);
			return node;
		}

		private static XElement Regex(DicomTag tag, string pattern)
		{
			return Element("regex", tag, new Dictionary<string, string> { { "pattern", pattern}, {"ignoreCase", "true" } });
		}

		private static XElement Not(XElement spec)
		{
			return new XElement(XName.Get("not"), spec);
		}

		private static XElement Element(string op, DicomTag tag, Dictionary<string, string> attributes)
		{
			var node = new XElement(XName.Get(op));
			node.SetAttributeValue(XName.Get("test"), string.Format("${0}", tag.VariableName));
			foreach (var kvp in attributes)
			{
				node.SetAttributeValue(XName.Get(kvp.Key), kvp.Value);
			}
			return node;
		}
	}
}
