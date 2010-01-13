using System;
using System.Collections.Generic;
using System.Xml;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	internal class XmlValidationBuilder
	{
		const string ScriptLanguage = "jscript";
		const string TagValidation = "validation";
		const string TagValidationRuleset = "validation-ruleset";
		const string TagValidationRule = "validation-rule";
		const string TagApplicabililtyRule = "applicability-rule";
		const string AttrEntityClass = "entityClass";
		const string AttrName = "name";

		private XmlDocument _rulesDoc;

		#region Public API

		/// <summary>
		/// Gets the complete rule set defined for the specified entity class, which includes all named rule sets.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <returns></returns>
		public ValidationRuleSet GetRuleset(Type entityClass)
		{
			InitializeOnce();

			// find node for the specified class
			var ruleSets = FindRulesetNodes(entityClass);

			// return a single rule set that combines all rule sets
			return ValidationRuleSet.Add(CollectionUtils.Map<XmlElement, ValidationRuleSet>(ruleSets, CompileRuleset));
		}

		/// <summary>
		/// Gets the specified named rule set.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEnumerable<XmlElement> GetRuleset(Type entityClass, string name)
		{
			InitializeOnce();

			// find node for the specified class
			var rulesNode = FindRulesetNode(entityClass, name);
			if (rulesNode != null)
			{
				// don't allow modification of our document
				var copy = (XmlElement)rulesNode.CloneNode(true);

				return new TypeSafeEnumerableWrapper<XmlElement>(copy.GetElementsByTagName(TagValidationRule));
			}

			// if not exist, return an empty list
			return new XmlElement[0];
		}

		/// <summary>
		/// Sets the custom rules for the specified application component class.  The rules are child elements of the specified parent node.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <param name="name"></param>
		/// <param name="parentNode"></param>
		public void SetRules(Type entityClass, string name, XmlElement parentNode)
		{
			InitializeOnce();

			// find node for specified class
			// if not exist, create
			var rulesetNode = FindRulesetNode(entityClass, name) ?? CreateRulesetNode(entityClass, name);

			// set inner XML from specified node
			rulesetNode.InnerXml = parentNode.InnerXml;
		}


		#endregion

		private bool Initialized
		{
			get { return _rulesDoc != null; }
		}

		private void InitializeOnce()
		{
			if (Initialized)
				return;

			var settings = new EntityValidationSettings();
			_rulesDoc = new XmlDocument { PreserveWhitespace = true };
			_rulesDoc.LoadXml(settings.CustomRulesXml);
		}

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

		private XmlNodeList FindRulesetNodes(Type entityClass)
		{
			return _rulesDoc.SelectNodes(
				string.Format("/{0}/{1}[@{2}='{3}']",
					TagValidation,
					TagValidationRuleset,
					AttrEntityClass,
					entityClass.FullName));
		}

		private XmlElement FindRulesetNode(Type entityClass, string name)
		{
			return (XmlElement)_rulesDoc.SelectSingleNode(
				string.Format("/{0}/{1}[@{2}='{3}' and @{4}='{5}']",
					TagValidation,
					TagValidationRuleset,
					AttrEntityClass,
					entityClass.FullName,
					AttrName,
					name));
		}

		private XmlElement CreateRulesetNode(Type entityClass, string name)
		{
			var rulesNode = _rulesDoc.CreateElement(TagValidationRuleset);
			rulesNode.SetAttribute(AttrEntityClass, entityClass.FullName);
			rulesNode.SetAttribute(AttrName, name);
			_rulesDoc.DocumentElement.AppendChild(rulesNode);
			return rulesNode;
		}
	}
}
