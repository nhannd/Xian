#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;
using ClearCanvas.Common.Configuration;
using System.Collections.Generic;

namespace ClearCanvas.Desktop.Validation
{
	[SettingsGroupDescription("Stores user-interface custom validation rules.")]
	[SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	internal sealed partial class ValidationRulesSettings
	{
		private XmlDocument _rulesDoc;

		private ValidationRulesSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		/// <summary>
		/// Gets the XML rules document.
		/// </summary>
		public XmlDocument RulesDocument
		{
			get
			{
				InitializeOnce();
				return _rulesDoc;
			}
		}

		//TODO (CR Sept 2010): we can now save these values via ApplicationSettingsExtensions

		/// <summary>
		/// Saves any changes made to the rules document to the specified settings store.
		/// </summary>
		/// <param name="settingsStore"></param>
		public void Save(ISettingsStore settingsStore)
		{
			// need to save changes to XML doc
			if (!Initialized)
				return;

			var sb = new StringBuilder();
			var writer = new XmlTextWriter(new StringWriter(sb)) { Formatting = Formatting.Indented };
			_rulesDoc.Save(writer);
			var xml = sb.ToString();

			// if value has not changed, nothing to save
			if (xml == this.CustomRulesXml)
				return;

			var values = new Dictionary<string, string> { { "CustomRulesXml", xml } };

			settingsStore.PutSettingsValues(
				new SettingsGroupDescriptor(this.GetType()),
				null,
				null,
				values);
		}

		private bool Initialized
		{
			get { return _rulesDoc != null; }
		}

		private void InitializeOnce()
		{
			if (Initialized)
				return;

			_rulesDoc = new XmlDocument {PreserveWhitespace = true};
			_rulesDoc.LoadXml(this.CustomRulesXml);
		}
	}
}
