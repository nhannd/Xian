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
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	public class MouseToolSettingsProfile : IXmlSerializable
	{
		private readonly Dictionary<string, Setting> _entries = new Dictionary<string, Setting>();

		public MouseToolSettingsProfile() {}

		public bool HasEntry(Type mouseImageViewerToolType)
		{
			Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
			string key = mouseImageViewerToolType.FullName;
			return _entries.ContainsKey(key);
		}

		public Setting this[Type mouseImageViewerToolType]
		{
			get
			{
				Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
				string key = mouseImageViewerToolType.FullName;
				if (!_entries.ContainsKey(key))
					_entries.Add(key, new Setting());
				return _entries[key];
			}
		}

		#region Static Profile Access

		private static MouseToolSettingsProfile _profile = null;

		public static MouseToolSettingsProfile Current
		{
			get
			{
				if (_profile == null)
				{
					XmlDocument xmlDocument = MouseToolSettings.Default.DefaultProfile;
					if (xmlDocument != null)
					{
						try
						{
							XmlSerializer deserializer = new XmlSerializer(typeof (MouseToolSettingsProfile));
							using (XmlNodeReader reader = new XmlNodeReader(xmlDocument))
							{
								_profile = (MouseToolSettingsProfile) deserializer.Deserialize(reader);
							}
						}
						catch (Exception ex)
						{
							Platform.Log(LogLevel.Warn, ex, "An exception was encountered while reading the mouse tool settings profile.");
						}
					}

					if (_profile == null)
						_profile = new MouseToolSettingsProfile();
				}
				return _profile;
			}
		}

		public static void SaveCurrentAsDefault()
		{
			if (_profile != null)
			{
				try
				{
					// suppress warning because CreateNavigator() returns null if the document is bad xml - which won't happen cause it's actually blank
					// ReSharper disable PossibleNullReferenceException
					MouseToolSettings settings = MouseToolSettings.Default;
					XmlDocument xmlDocument = new XmlDocument();
					XmlSerializer serializer = new XmlSerializer(typeof (MouseToolSettingsProfile));
					using (XmlWriter writer = xmlDocument.CreateNavigator().AppendChild())
					{
						serializer.Serialize(writer, _profile);
					}
					settings.DefaultProfile = xmlDocument;
					settings.Save();
					// ReSharper restore PossibleNullReferenceException
				}
				catch (Exception ex)
				{
					Platform.Log(LogLevel.Warn, ex, "An exception was encountered while writing the mouse tool settings profile.");
				}
			}
		}

		#endregion

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
					string key = reader.GetAttribute("Type");
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
				writer.WriteStartElement(typeof (Setting).Name);
				writer.WriteAttributeString("Type", settingEntry.Key);
				((IXmlSerializable) settingEntry.Value).WriteXml(writer);
				writer.WriteEndElement();
			}
		}

		#endregion

		#region Setting Class

		public class Setting : IXmlSerializable
		{
			private XMouseButtons? _mouseButton = null;
			private bool? _initiallyActive = null;

			internal Setting() {}

			public bool? InitiallyActive
			{
				get { return _initiallyActive; }
				set { _initiallyActive = value; }
			}

			public XMouseButtons? MouseButton
			{
				get { return _mouseButton; }
				set { _mouseButton = value; }
			}

			#region IXmlSerializable Members

			XmlSchema IXmlSerializable.GetSchema()
			{
				return null;
			}

			void IXmlSerializable.ReadXml(XmlReader reader)
			{
				_initiallyActive = null;
				_mouseButton = null;

				reader.MoveToContent();

				bool isEmpty = reader.IsEmptyElement;
				reader.ReadStartElement();
				if (!isEmpty)
				{
					while (reader.MoveToContent() == XmlNodeType.Element)
					{
						switch (reader.Name)
						{
							case "InitiallyActive":
								_initiallyActive = ParseBoolean(reader.ReadElementString());
								break;
							case "MouseButton":
								_mouseButton = ParseXMouseButtons(reader.ReadElementString());
								break;
						}
					}

					reader.ReadEndElement();
				}
			}

			void IXmlSerializable.WriteXml(XmlWriter writer)
			{
				if (_initiallyActive.HasValue)
					writer.WriteElementString("InitiallyActive", _initiallyActive.Value.ToString());

				if (_mouseButton.HasValue)
					writer.WriteElementString("MouseButton", _mouseButton.Value.ToString());
			}

			private static bool? ParseBoolean(string value)
			{
				bool result;
				if (!string.IsNullOrEmpty(value) && bool.TryParse(value, out result))
					return result;
				return null;
			}

			private static XMouseButtons? ParseXMouseButtons(string value)
			{
				if (string.IsNullOrEmpty(value))
					return null;

				try
				{
					return (XMouseButtons) Enum.Parse(typeof (XMouseButtons), value, true);
				}
				catch (Exception)
				{
					return null;
				}
			}

			#endregion
		}

		#endregion
	}
}