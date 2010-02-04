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

using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace ClearCanvas.ImageViewer.Tools.Standard
{
	public class ToolSettingsProfile : IXmlSerializable
	{
		private readonly Dictionary<string, Setting> _entries = new Dictionary<string, Setting>();

		public ToolSettingsProfile() {}

		public bool HasSetting(string modality)
		{
			return _entries.ContainsKey(modality);
		}

		public Setting this[string modality]
		{
			get
			{
				if (!_entries.ContainsKey(modality))
					_entries.Add(modality, new Setting());
				return _entries[modality];
			}
		}

		#region IXmlSerializable Members

		XmlSchema IXmlSerializable.GetSchema()
		{
			return null;
		}

		void IXmlSerializable.ReadXml(XmlReader reader)
		{
			_entries.Clear();

			reader.MoveToContent();

			bool isEmptyElement = reader.IsEmptyElement;
			reader.ReadStartElement();
			if (!isEmptyElement)
			{
				while (reader.MoveToContent() == XmlNodeType.Element && reader.Name == typeof (Setting).Name)
				{
					string key = reader.GetAttribute("Modality");
					Setting value = new Setting();
					((IXmlSerializable) value).ReadXml(reader);
					_entries.Add(key, value);
				}

				reader.ReadEndElement();
			}
		}

		void IXmlSerializable.WriteXml(XmlWriter writer)
		{
			foreach (KeyValuePair<string, Setting> settingEntry in _entries)
			{
				if (!settingEntry.Value.RequiresSerialization)
					continue;

				writer.WriteStartElement(typeof (Setting).Name);
				writer.WriteAttributeString("Modality", settingEntry.Key);
				((IXmlSerializable) settingEntry.Value).WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion

		#region Setting Class

		public class Setting : IXmlSerializable
		{
			private bool? _autoCineMultiframes = null;

			internal Setting() {}

			public bool? AutoCineMultiframes
			{
				get { return _autoCineMultiframes; }
				set { _autoCineMultiframes = value; }
			}

			internal bool RequiresSerialization
			{
				get { return _autoCineMultiframes.HasValue; }
			}

			#region IXmlSerializable Members

			XmlSchema IXmlSerializable.GetSchema()
			{
				return null;
			}

			void IXmlSerializable.ReadXml(XmlReader reader)
			{
				_autoCineMultiframes = null;

				reader.MoveToContent();

				bool isEmpty = reader.IsEmptyElement;
				reader.ReadStartElement();
				if (!isEmpty)
				{
					while (reader.MoveToContent() == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "AutoCineMultiframes":
								_autoCineMultiframes = ParseBoolean(reader.ReadElementString());
								break;
						}
					}

					reader.ReadEndElement();
				}
			}

			void IXmlSerializable.WriteXml(XmlWriter writer)
			{
				if (_autoCineMultiframes.HasValue)
					writer.WriteElementString("AutoCineMultiframes", _autoCineMultiframes.Value.ToString());
			}

			private static bool? ParseBoolean(string value)
			{
				bool result;
				if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
					return result;
				return null;
			}

			#endregion
		}

		#endregion
	}
}