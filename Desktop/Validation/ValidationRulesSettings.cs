#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
