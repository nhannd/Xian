#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Configuration;
using System.Xml;
using ClearCanvas.Common.Utilities;

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

		const string TagValidation = "validation";
		const string TagValidationRuleset = "validation-ruleset";
		const string TagValidationRule = "validation-rule";
		const string AttrClass = "class";

		private readonly ISettingsStore _settingsStore;

		/// <summary>
		/// Constructor
		/// </summary>
		private XmlValidationManager()
		{
			try
			{
				_settingsStore = (ISettingsStore)(new SettingsStoreExtensionPoint()).CreateExtension();
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
			//TODO (CR Sept 2010): If ValidationRulesSettings used ApplicationSettingsExtensions to save, we wouldn't need this.
			get { return _settingsStore != null; }
		}

		/// <summary>
		/// Gets the custom rules for the specified application component class, as a set of XML elements where each element represents a rule.
		/// </summary>
		/// <param name="componentClass"></param>
		/// <returns></returns>
		public IEnumerable<XmlElement> GetRules(Type componentClass)
		{
			CheckSupported();

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
			ValidationRulesSettings.Default.Save(_settingsStore);
		}

		#endregion

		private void CheckSupported()
		{
			if (!IsSupported)
				throw new NotSupportedException("XML validation rules are not supported because there is no configuration store.");
		}

		private static XmlElement FindRulesNode(Type componentClass)
		{

			return (XmlElement)ValidationRulesSettings.Default.RulesDocument.SelectSingleNode(
				string.Format("/{0}/{1}[@{2}='{3}']", TagValidation, TagValidationRuleset, AttrClass, componentClass.FullName));
		}

		private static XmlElement CreateRulesNode(Type componentClass)
		{
			var rulesDoc = ValidationRulesSettings.Default.RulesDocument;
			var rulesNode = rulesDoc.CreateElement(TagValidationRuleset);
			rulesNode.SetAttribute(AttrClass, componentClass.FullName);
			rulesDoc.DocumentElement.AppendChild(rulesNode);
			return rulesNode;
		}
	}
}
