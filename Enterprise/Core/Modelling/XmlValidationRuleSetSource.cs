using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	/// <summary>
	/// Abstract base class for implementations of <see cref="IValidationRuleSetSource"/> based on XML validation documents.
	/// </summary>
	public abstract class XmlValidationRuleSetSource : IValidationRuleSetSource
	{
		const string ScriptLanguage = "jscript";
		const string TagValidation = "validation";
		const string TagValidationRule = "validation-rule";
		const string TagApplicabililtyRule = "applicability-rule";
		const string TagValidationRuleset = "validation-ruleset";
		const string AttrClass = "class";

		#region Implementation of IXmlValidationRuleSetSource

		public ValidationRuleSet GetRuleSet(string @class)
		{
			var ruleSetNodes = FindRulesetNodes(@class);

			// return a single rule set that combines all rule sets
			return ValidationRuleSet.Add(CollectionUtils.Map<XmlElement, ValidationRuleSet>(ruleSetNodes, CompileRuleset));
		}

		#endregion

		protected abstract XmlDocument RuleSetDocument { get; }

		private static ValidationRuleSet CompileRuleset(XmlElement rulesetNode)
		{
			var compiler = new XmlSpecificationCompiler(ScriptLanguage);
			var rules = CollectionUtils.Map<XmlElement, ISpecification>(
				rulesetNode.GetElementsByTagName(TagValidationRule), compiler.Compile);

			var applicabilityRuleNode = CollectionUtils.FirstElement(rulesetNode.GetElementsByTagName(TagApplicabililtyRule));
			return applicabilityRuleNode != null ?
				new ValidationRuleSet(rules, compiler.Compile((XmlElement)applicabilityRuleNode))
				: new ValidationRuleSet(rules);
		}

		private XmlNodeList FindRulesetNodes(string @class)
		{
			return RuleSetDocument.SelectNodes(
				string.Format("/{0}/{1}[@{2}='{3}']",
					TagValidation,
					TagValidationRuleset,
					AttrClass,
					@class));
		}
	}
}
