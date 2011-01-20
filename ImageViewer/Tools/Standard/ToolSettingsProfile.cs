#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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