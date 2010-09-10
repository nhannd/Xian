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
using System.Xml;
using System.Collections.Generic;

namespace ClearCanvas.Common.Configuration
{
	internal class ConfigurationFileReader
	{
		private readonly XmlDocument _document;

		public ConfigurationFileReader(string fileName)
			: this(new XmlDocument())
		{
			_document.Load(fileName);
		}

		public ConfigurationFileReader(XmlDocument document)
		{
			_document = document;
		}

		public IDictionary<string, string> GetSettingsValues(ConfigurationSectionPath path)
		{
			var values = new Dictionary<string, string>();

			if (_document.DocumentElement != null)
			{
				var element = _document.DocumentElement.SelectSingleNode(path) as XmlElement;
				if (element != null)
				{
					foreach (XmlElement setting in element.ChildNodes)
					{
						var nameAttribute = setting.Attributes["name"];
						if (nameAttribute == null)
							continue;

						var name = nameAttribute.Value;
						if (String.IsNullOrEmpty(name))
							continue;

						var valueNode = setting.SelectSingleNode("value");
						if (valueNode == null)
							continue;

						var serializeAsAttribute = setting.Attributes["serializeAs"];
						var serializeAsValue = serializeAsAttribute != null ? serializeAsAttribute.Value : String.Empty;
						var serializeAs = SystemConfigurationHelper.GetSerializeAs(serializeAsValue);
						values.Add(name, SystemConfigurationHelper.GetElementValue(valueNode, serializeAs));
					}
				}
			}

			return values;
		}

		public IDictionary<string, string> GetSettingsValues(Type settingsClass, SettingScope scope)
		{
			return GetSettingsValues(new ConfigurationSectionPath(settingsClass, scope));
		}

		public IDictionary<string, string> GetSettingsValues(Type settingsClass)
		{
			var appSettings = GetSettingsValues(settingsClass, SettingScope.Application);
			var userSettings = GetSettingsValues(settingsClass, SettingScope.User);
			foreach (var setting in userSettings)
				appSettings[setting.Key] = setting.Value;

			return appSettings;
		}
	}
}
