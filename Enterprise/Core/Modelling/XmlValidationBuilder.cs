using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using ClearCanvas.Common.Specifications;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Enterprise.Core.Modelling
{
	internal class XmlValidationBuilder
	{
		const string ScriptLanguage = "jscript";
		const string DocumentName = "ClearCanvas.Enterprise.Core.Modelling.CustomEntityValidationRules.xml";
		const string TagValidation = "validation";
		const string TagValidationRuleset = "validation-ruleset";
		const string TagValidationRule = "validation-rule";
		const string TagApplicabililtyRule = "applicability-rule";
		const string AttrEntityClass = "entityClass";
		const string AttrName = "name";

		private readonly IConfigurationStore _configStore;
		private XmlDocument _rulesDoc;

		/// <summary>
		/// Constructor
		/// </summary>
		internal XmlValidationBuilder()
		{
			try
			{
				_configStore = ConfigurationStoreFactory.GetDefaultStore();
			}
			catch (NotSupportedException e)
			{
				Platform.Log(LogLevel.Debug, e);
			}
		}

		#region Public API

		/// <summary>
		/// Gets a value indicating whether custom validation rules are supported.
		/// </summary>
		public bool IsSupported
		{
			get { return _configStore != null; }
		}

		/// <summary>
		/// Gets the complete rule set defined for the specified entity class, which includes all named rule sets.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <returns></returns>
		public ValidationRuleSet GetRuleset(Type entityClass)
		{
			CheckSupported();
			InitializeOnce();

			// find node for the specified class
			var ruleSets = FindRulesetNodes(entityClass);

			// return a single rule set that combines all rule sets
			return ValidationRuleSet.Combine(CollectionUtils.Map<XmlElement, ValidationRuleSet>(ruleSets, CompileRuleset));
		}

		/// <summary>
		/// Gets the specified named rule set.
		/// </summary>
		/// <param name="entityClass"></param>
		/// <param name="name"></param>
		/// <returns></returns>
		public IEnumerable<XmlElement> GetRuleset(Type entityClass, string name)
		{
			CheckSupported();
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
			CheckSupported();
			InitializeOnce();

			// find node for specified class
			// if not exist, create
			var rulesetNode = FindRulesetNode(entityClass, name) ?? CreateRulesetNode(entityClass, name);

			// set inner XML from specified node
			rulesetNode.InnerXml = parentNode.InnerXml;
		}

		/// <summary>
		/// Saves all changes made to the document via <see cref="SetRules"/>.
		/// </summary>
		public void Save()
		{
			CheckSupported();

			if (!Initialized)
				return;

			var sb = new StringBuilder();
			var writer = new XmlTextWriter(new StringWriter(sb)) {Formatting = Formatting.Indented};
			_rulesDoc.Save(writer);

			_configStore.PutDocument(
				DocumentName,
				this.GetType().Assembly.GetName().Version,
				null,
				null,
				new StringReader(sb.ToString())
				);
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

			_rulesDoc = new XmlDocument {PreserveWhitespace = true};
			try
			{
				using (var reader = _configStore.GetDocument(
					DocumentName, this.GetType().Assembly.GetName().Version, null, null))
				{
					_rulesDoc.Load(reader);
				}
			}
			catch (ConfigurationDocumentNotFoundException e)
			{
				// no validation document exists yet
				// this is not an error, but might be useful to know this for debugging
				Platform.Log(LogLevel.Debug, e);

				// create an empty document
				var resolver = new ResourceResolver(this.GetType().Assembly);
				using(var resource = resolver.OpenResource(DocumentName))
				{
					_rulesDoc.Load(resource);
				}
			}
		}

		private void CheckSupported()
		{
			if (!IsSupported)
				throw new NotSupportedException("XML validation rules are not supported because there is no configuration store.");
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
