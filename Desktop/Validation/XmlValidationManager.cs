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
	public class XmlValidationManager
	{
		private static readonly XmlValidationManager _instance = new XmlValidationManager();

		/// <summary>
		/// Gets the singleton instance of this class.
		/// </summary>
		public static XmlValidationManager Instance { get { return _instance; } }

		const string _documentName = "ClearCanvas.Desktop.Validation.CustomRules.xml";
		const string tagValidation = "validation";
		const string tagValidationRules = "validation-rules";
		const string tagValidationRule = "validation-rule";
		const string attrComponentClass = "componentClass";

		private XmlDocument _rulesDoc;
		private IConfigurationStore _configStore;

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

		public bool IsSupported
		{
			get { return _configStore != null; }
		}

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

				return new TypeSafeEnumerableWrapper<XmlElement>(copy.GetElementsByTagName(tagValidationRule));
			}

			// if not exist, return an empty list
			return new XmlElement[0];
		}

		public void SetRules(Type componentClass, XmlElement parentNode)
		{
			CheckSupported();
			InitializeOnce();

			// find node for specified class
			var rulesNode = FindRulesNode(componentClass);

			// if not exist, create
			if (rulesNode == null)
				rulesNode = CreateRulesNode(componentClass);

			// set inner XML from specified node
			rulesNode.InnerXml = parentNode.InnerXml;
		}

		public void Save()
		{
			CheckSupported();

			if (!Initialized)
				return;

			StringBuilder sb = new StringBuilder();
			XmlTextWriter writer = new XmlTextWriter(new StringWriter(sb));
			writer.Formatting = System.Xml.Formatting.Indented;
			_rulesDoc.Save(writer);

			_configStore.PutDocument(
				_documentName,
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

			_rulesDoc = new XmlDocument();
			_rulesDoc.PreserveWhitespace = true;
			try
			{
				using (var reader = _configStore.GetDocument(
					_documentName, this.GetType().Assembly.GetName().Version, null, null))
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
				_rulesDoc.LoadXml(string.Format("<{0}/>", tagValidation));
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
				string.Format("/{0}/{1}[@{2}='{3}']", tagValidation, tagValidationRules, attrComponentClass, componentClass.FullName));
		}

		private XmlElement CreateRulesNode(Type componentClass)
		{
			var rulesNode = _rulesDoc.CreateElement(tagValidationRules);
			rulesNode.SetAttribute(attrComponentClass, componentClass.FullName);
			_rulesDoc.DocumentElement.AppendChild(rulesNode);
			return rulesNode;
		}
	}
}
