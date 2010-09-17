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
using System.ComponentModel;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.BaseTools
{
	public class MouseToolSettingsProfile : IXmlSerializable
	{
		private readonly Dictionary<string, Setting> _entries;
		private readonly Dictionary<string, string> _toolActivationActionIdMap;

		public MouseToolSettingsProfile()
		{
			_toolActivationActionIdMap = new Dictionary<string, string>();
			_entries = new Dictionary<string, Setting>();
		}

		private MouseToolSettingsProfile(MouseToolSettingsProfile original)
		{
			_toolActivationActionIdMap = new Dictionary<string, string>(original._toolActivationActionIdMap);
			_entries = new Dictionary<string, Setting>(original._entries.Count);
			foreach (KeyValuePair<string, Setting> entry in original._entries)
				_entries.Add(entry.Key, new Setting(entry.Value));
		}

		public bool HasEntry(Type mouseImageViewerToolType)
		{
			Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
			string key = mouseImageViewerToolType.FullName;
			return this.HasEntryCore(key);
		}

		protected bool HasEntryCore(string key)
		{
			return _entries.ContainsKey(key);
		}

		public Setting this[Type mouseImageViewerToolType]
		{
			get
			{
				Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
				Platform.CheckTrue(typeof (MouseImageViewerTool).IsAssignableFrom(mouseImageViewerToolType), "mouseImageViewerToolType should be a MouseImageViewerTool type.");
				string key = mouseImageViewerToolType.FullName;
				return GetSettingCore(key);
			}
		}

		protected Setting GetSettingCore(string key)
		{
			{
				if (!_entries.ContainsKey(key))
					_entries.Add(key, new Setting());
				return _entries[key];
			}
		}

		//TODO (CR Sept 2010): name of these methods is awkward, maybe because it's ActivationAction.  Can we use SelectAction instead?
		public bool IsRegisteredMouseToolActivationAction(string actionId)
		{
			Platform.CheckForEmptyString(actionId, "toolActivationActionId");
			return _toolActivationActionIdMap.ContainsKey(actionId);
		}

		public bool HasEntryByActivationActionId(string toolActivationActionId)
		{
			Platform.CheckForEmptyString(toolActivationActionId, "toolActivationActionId");
			if (!IsRegisteredMouseToolActivationAction(toolActivationActionId))
				return false;
			string key = _toolActivationActionIdMap[toolActivationActionId];
			return this.HasEntryCore(key);
		}

		//TODO (CR Sept 2010): GetSetting instead of GetEntry?
		public Setting GetEntryByActivationActionId(string toolActivationActionId)
		{
			Platform.CheckForEmptyString(toolActivationActionId, "toolActivationActionId");
			Platform.CheckTrue(IsRegisteredMouseToolActivationAction(toolActivationActionId), "hat power is unknown");
			string key = _toolActivationActionIdMap[toolActivationActionId];
			return GetSettingCore(key);
		}

		internal void RegisterActivationActionId(string actionId, Type mouseImageViewerToolType)
		{
			Platform.CheckForEmptyString(actionId, "actionId");
			Platform.CheckForNullReference(mouseImageViewerToolType, "mouseImageViewerToolType");
			Platform.CheckTrue(typeof(MouseImageViewerTool).IsAssignableFrom(mouseImageViewerToolType), "mouseImageViewerToolType should be a MouseImageViewerTool type.");
			string typeName = mouseImageViewerToolType.FullName;

			Platform.CheckFalse(_toolActivationActionIdMap.ContainsKey(actionId) && _toolActivationActionIdMap[actionId] != typeName, "The given actionId has already been registered to a different tool type!");
			if (!_toolActivationActionIdMap.ContainsKey(actionId))
				_toolActivationActionIdMap.Add(actionId, typeName);
		}

		public MouseToolSettingsProfile Clone()
		{
			return new MouseToolSettingsProfile(this);
		}

		#region Static Profile Access

		private static event EventHandler _currentProfileChanged;
		private static MouseToolSettingsProfile _profile = null;

		public static MouseToolSettingsProfile Current
		{
			get
			{
				if (_profile == null)
				{
					_profile = MouseToolSettings.Default.DefaultProfile;
					if (_profile == null)
						_profile = new MouseToolSettingsProfile();
				}
				return _profile;
			}
			set
			{
				Platform.CheckForNullReference(value, "value");
				if (_profile != value)
				{
					_profile = value;
					EventsHelper.Fire(_currentProfileChanged, null, EventArgs.Empty);
				}
			}
		}

		public static event EventHandler CurrentProfileChanged
		{
			add { _currentProfileChanged += value; }
			remove { _currentProfileChanged -= value; }
		}

		//TODO (CR Sept 2010): why not just do this when Current is set?
		public static void SaveCurrentAsDefault()
		{
			if (_profile != null)
			{
				try
				{
					MouseToolSettings settings = MouseToolSettings.Default;
					settings.DefaultProfile = _profile;
					settings.Save();
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
				if (settingEntry.Value.IsEmpty)
					continue;
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
			private ModifierFlags? _defaultMouseButtonModifiers = null;
			private XMouseButtons? _defaultMouseButton = null;
			private XMouseButtons? _mouseButton = null;
			private bool? _initiallyActive = null;

			internal Setting() {}

			internal Setting(Setting original)
			{
				_defaultMouseButtonModifiers = original._defaultMouseButtonModifiers;
				_defaultMouseButton = original._defaultMouseButton;
				_mouseButton = original._mouseButton;
				_initiallyActive = original._initiallyActive;
			}

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

			public XMouseButtons? DefaultMouseButton
			{
				get { return _defaultMouseButton; }
				set { _defaultMouseButton = value; }
			}

			public ModifierFlags? DefaultMouseButtonModifiers
			{
				get { return _defaultMouseButtonModifiers; }
				set { _defaultMouseButtonModifiers = value; }
			}

			public bool IsEmpty
			{
				get
				{
					return !(_initiallyActive.HasValue
					         || _mouseButton.HasValue
					         || _defaultMouseButton.HasValue
					         || _defaultMouseButtonModifiers.HasValue);
				}
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
								_initiallyActive = Parse<bool>(reader.ReadElementString());
								break;
							case "MouseButton":
								_mouseButton = Parse<XMouseButtons>(reader.ReadElementString());
								break;
							case "DefaultMouseButton":
								_defaultMouseButton = Parse<XMouseButtons>(reader.ReadElementString());
								break;
							case "DefaultMouseButtonModifiers":
								_defaultMouseButtonModifiers = Parse<ModifierFlags>(reader.ReadElementString());
								break;
						}
					}

					reader.ReadEndElement();
				}
			}

			void IXmlSerializable.WriteXml(XmlWriter writer)
			{
				if (_initiallyActive.HasValue)
					writer.WriteElementString("InitiallyActive", Format(_initiallyActive.Value));

				if (_mouseButton.HasValue)
					writer.WriteElementString("MouseButton", Format(_mouseButton.Value));

				if (_defaultMouseButton.HasValue)
					writer.WriteElementString("DefaultMouseButton", Format(_defaultMouseButton.Value));

				if (_defaultMouseButtonModifiers.HasValue)
					writer.WriteElementString("DefaultMouseButtonModifiers", Format(_defaultMouseButtonModifiers.Value));
			}

			private static string Format<T>(T value) where T : struct
			{
				return TypeDescriptor.GetConverter(typeof (T)).ConvertToInvariantString(value);
			}

			private static T? Parse<T>(string value) where T : struct
			{
				if (string.IsNullOrEmpty(value))
					return null;

				try
				{
					return (T) TypeDescriptor.GetConverter(typeof (T)).ConvertFromInvariantString(value);
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