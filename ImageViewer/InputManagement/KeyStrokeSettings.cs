using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using System.Configuration;
using System.Xml.Serialization;

namespace ClearCanvas.ImageViewer.InputManagement
{
	[Serializable]
	public class KeyStrokeSettings : ExtensionSettings
	{
		public KeyStrokeSettings()
		{
		}

		[UserScopedSetting]
		public List<KeyStrokeSetting> Settings
		{
			get { return (List<KeyStrokeSetting>)this["Settings"]; }
			set { this["Settings"] = value; }
		}
	}

	[Serializable]
	public class KeyStrokeSetting
	{
		private XKeys _keyStroke;
		private string _actionId;
		private string _actionPath;

		public KeyStrokeSetting()
		{
		}

		public XKeys KeyStroke
		{
			get { return _keyStroke; }
			set { _keyStroke = value; }
		}

		public string ActionId
		{
			get { return _actionId; }
			set { _actionId = value; }
		}

		public string ActionPath
		{
			get { return _actionPath; }
			set { _actionPath = value; }
		}
	}
}
