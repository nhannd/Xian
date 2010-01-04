using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using System.Xml;
using ClearCanvas.Common.Utilities;
using System.IO;

namespace ClearCanvas.Desktop.Validation
{
	/// <summary>
	/// Manages an XML document containing custom validation rules.
	/// </summary>
	public class XmlValidationManager
	{
		private static readonly XmlValidationManager _instance = new XmlValidationManager();

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static XmlValidationManager Instance { get { return _instance; } }

		const string DocumentName = "ClearCanvas.Desktop.Validation.CustomRules.xml";
		const string TagValidation = "validation";
		const string TagValidationRules = "validation-rules";
		const string TagValidationRule = "validation-rule";
		const string AttrComponentClass = "componentClass";

		private readonly IConfigurationStore _configStore;
		private XmlDocument _rulesDoc;

		/// <summary>
		/// Constructor
		/// </summary>
		private XmlValidationManager()
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
		/// Gets the custom rules for the specified application component class, as a set of XML elements where each element represents a rule.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public IEnumerable<XmlElement> GetRules(Type componentClass)
		{
			CheckSupported();
			InitializeOnce();

			// find node for the specified class
			var rulesNode = FindRulesNode(componentClass);
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
		/// <param name="componentClass"></param>
		/// <param name="parentNode"></param>
		public void SetRules(Type componentClass, XmlElement parentNode)
		{
			CheckSupported();
			InitializeOnce();

			// find node for specified class
			// if not exist, create
			var rulesNode = FindRulesNode(componentClass) ?? CreateRulesNode(componentClass);

			// set inner XML from specified node
			rulesNode.InnerXml = parentNode.InnerXml;
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
				_rulesDoc.LoadXml(string.Format("<{0}/>", TagValidation));
			}
		}

		private void CheckSupported()
		{
			if (!IsSupported)
				throw new NotSupportedException("XML validation rules are not supported because there is no configuration store.");
		}

		private XmlElement FindRulesNode(Type componentClass)
		{
			return (XmlElement)_rulesDoc.SelectSingleNode(
				string.Format("/{0}/{1}[@{2}='{3}']", TagValidation, TagValidationRules, AttrComponentClass, componentClass.FullName));
		}

		private XmlElement CreateRulesNode(Type componentClass)
		{
			var rulesNode = _rulesDoc.CreateElement(TagValidationRules);
			rulesNode.SetAttribute(AttrComponentClass, componentClass.FullName);
			_rulesDoc.DocumentElement.AppendChild(rulesNode);
			return rulesNode;
		}
	}
}
