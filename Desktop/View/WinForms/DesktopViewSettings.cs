#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common.Configuration;
using System.Xml;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[SettingsGroupDescription("Stores window positions for the application, so they can be restored the next time it runs")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class DesktopViewSettings
	{
		#region DesktopWindowState class 

		private class DesktopWindowState : IEquatable<DesktopWindowState>
		{
			public DesktopWindowState(string windowName, Rectangle windowRegion, FormWindowState windowState)
			{
				WindowName = windowName ?? "";
				WindowRegion = windowRegion;
				WindowState = windowState;
			}

			public readonly string WindowName;
			public readonly Rectangle WindowRegion;
			public readonly FormWindowState WindowState;

			public static DesktopWindowState FromXmlElement(XmlElement element)
			{
				string windowName = element.GetAttribute("name") ?? "";

				TypeConverter converter = TypeDescriptor.GetConverter(typeof (Rectangle));
				Rectangle windowRegion = (Rectangle)converter.ConvertFromString(element.GetAttribute("region"));

				converter = TypeDescriptor.GetConverter(typeof(FormWindowState));
				FormWindowState windowState = (FormWindowState)converter.ConvertFromString(element.GetAttribute("state"));

				return new DesktopWindowState(windowName, windowRegion, windowState);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				element.SetAttribute("name", WindowName);

				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				element.SetAttribute("region", converter.ConvertToString(WindowRegion));

				converter = TypeDescriptor.GetConverter(typeof(FormWindowState));
				element.SetAttribute("state", converter.ConvertToString(WindowState));
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;
				if (obj is DesktopWindowState)
					return Equals((DesktopWindowState)obj);

				return false;
			}

			#region IEquatable<DesktopWindowState> Members

			public bool Equals(DesktopWindowState other)
			{
				return other.WindowName.Equals(WindowName);
			}

			#endregion
		}

		#endregion

		#region ScreenConfiguration class

		private class ScreenConfiguration : IEquatable<ScreenConfiguration>
		{
			private ScreenConfiguration(Rectangle screenRegion)
			{
				ScreenRegion = screenRegion;
			}

			public readonly Rectangle ScreenRegion;

			public static ScreenConfiguration FromXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				Rectangle region = (Rectangle)converter.ConvertFromString(element.GetAttribute("region"));

				return new ScreenConfiguration(region);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				element.SetAttribute("region", converter.ConvertToString(ScreenRegion));
			}

			public static ScreenConfiguration[] Get()
			{
				List<ScreenConfiguration> configurations = new List<ScreenConfiguration>();

				foreach (Screen screen in Screen.AllScreens)
					configurations.Add(new ScreenConfiguration(screen.Bounds));

				return configurations.ToArray();
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;
				if (obj is ScreenConfiguration)
					return Equals((ScreenConfiguration)obj);
			
				return false;
			}

			#region IEquatable<ScreenConfiguration> Members

			public bool Equals(ScreenConfiguration other)
			{
				return other.ScreenRegion.Equals(ScreenRegion);
			}

			#endregion
		}

		#endregion

		#region DesktopWindowSetting class

		private class DesktopWindowSetting : IEquatable<DesktopWindowSetting>
		{
			private DesktopWindowSetting(Rectangle virtualScreenRectangle, ScreenConfiguration[] screenConfigurations)
				: this(virtualScreenRectangle, screenConfigurations, new List<DesktopWindowState>())
			{
			}

			private DesktopWindowSetting(Rectangle virtualScreenRectangle, ScreenConfiguration[] screenConfigurations, List<DesktopWindowState> desktopWindowStates)
			{
				VirtualScreenRectangle = virtualScreenRectangle;
				ScreenConfigurations = screenConfigurations;
				DesktopWindowStates = desktopWindowStates;
			}
			
			public readonly Rectangle VirtualScreenRectangle;
			public readonly ScreenConfiguration[] ScreenConfigurations;
			private readonly List<DesktopWindowState> DesktopWindowStates;

			public static DesktopWindowSetting FromXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof (Rectangle));
				Rectangle virtualScreenRectangle = (Rectangle)converter.ConvertFromString(element.GetAttribute("virtual-screen-rectangle"));

				List<ScreenConfiguration> screenConfigurations = new List<ScreenConfiguration>();
				foreach (XmlElement screenConfigurationElement in element.SelectNodes("screen-configurations/screen-configuration"))
					screenConfigurations.Add(ScreenConfiguration.FromXmlElement(screenConfigurationElement));

				List<DesktopWindowState> desktopWindowStates = new List<DesktopWindowState>();
				foreach (XmlElement desktopWindowStateElement in element.SelectNodes("desktop-window-states/desktop-window-state"))
					desktopWindowStates.Add(DesktopWindowState.FromXmlElement(desktopWindowStateElement));

				return new DesktopWindowSetting(virtualScreenRectangle, screenConfigurations.ToArray(), desktopWindowStates);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				element.SetAttribute("virtual-screen-rectangle", converter.ConvertToString(VirtualScreenRectangle));

				XmlElement screenConfigurationsElement = element.OwnerDocument.CreateElement("screen-configurations");
				element.AppendChild(screenConfigurationsElement);

				foreach (ScreenConfiguration screenConfiguration in ScreenConfigurations)
				{
					XmlElement screenConfigurationElement = element.OwnerDocument.CreateElement("screen-configuration");
					screenConfiguration.UpdateXmlElement(screenConfigurationElement);
					screenConfigurationsElement.AppendChild(screenConfigurationElement);
				}

				XmlElement desktopWindowStatesElement = element.OwnerDocument.CreateElement("desktop-window-states");
				element.AppendChild(desktopWindowStatesElement);

				foreach (DesktopWindowState desktopWindowState in DesktopWindowStates)
				{
					XmlElement desktopWindowStateElement = element.OwnerDocument.CreateElement("desktop-window-state");
					desktopWindowState.UpdateXmlElement(desktopWindowStateElement);
					desktopWindowStatesElement.AppendChild(desktopWindowStateElement);
				}
			}

			public bool Bad
			{
				get { return DesktopWindowStates.Count == 0 || ScreenConfigurations.Length == 0; }	
			}

			public DesktopWindowState this[string name]
			{
				get
				{
					string lookup = name ?? "";
					return DesktopWindowStates.Find(delegate(DesktopWindowState test) { return test.WindowName == lookup; });
				}
			}

			public void Update(DesktopWindowState state)
			{
				int existingStateIndex = DesktopWindowStates.IndexOf(state);
				if (existingStateIndex >= 0)
					DesktopWindowStates.RemoveAt(existingStateIndex);

				DesktopWindowStates.Add(state);
			}

			public static DesktopWindowSetting Get()
			{
				return new DesktopWindowSetting(SystemInformation.VirtualScreen, ScreenConfiguration.Get());
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;
				if (obj is DesktopWindowSetting)
					return Equals((DesktopWindowSetting) obj);

				return false;
			}

			#region IEquatable<DesktopWindowSetting> Members

			public bool Equals(DesktopWindowSetting other)
			{
				if (!other.VirtualScreenRectangle.Equals(VirtualScreenRectangle))
					return false;

				if (other.ScreenConfigurations.Length != ScreenConfigurations.Length)
					return false;

				int i = 0;
				foreach (ScreenConfiguration configuration in other.ScreenConfigurations)
				{
					if (!configuration.Equals(ScreenConfigurations[i++]))
						return false;
				}

				return true;
			}

			#endregion
		}

		#endregion

		private DesktopViewSettings()
		{
			ApplicationSettingsRegistry.Instance.RegisterInstance(this);
		}

		#region Private Methods

		private DesktopWindowSetting GetDesktopWindowSetting()
		{
			DesktopWindowSetting currentSetting = DesktopWindowSetting.Get();

			foreach(DesktopWindowSetting setting in GetDesktopWindowSettings())
			{
				if (currentSetting.Equals(setting))
					return setting;
			}

			return currentSetting;
		}

		private List<DesktopWindowSetting> GetDesktopWindowSettings()
		{
			List<DesktopWindowSetting> settings = new List<DesktopWindowSetting>();
			
			foreach (XmlElement element in DesktopWindowSettingsXml.SelectNodes("desktop-window-settings/desktop-window-setting"))
			{
				try
				{
					DesktopWindowSetting setting = DesktopWindowSetting.FromXmlElement(element);
					int existingIndex = settings.IndexOf(setting);
					if (existingIndex >= 0)
						settings.RemoveAt(existingIndex);

					settings.Add(setting);
				}
				catch (Exception e)
				{
					Platform.Log(LogLevel.Warn, e);
				}
			}

			return settings;
		}

		private void SaveDesktopWindowSettings(IEnumerable<DesktopWindowSetting> desktopWindowSettings)
		{
			XmlDocument document = new XmlDocument();
			XmlElement settingsElement = document.CreateElement("desktop-window-settings");
			document.AppendChild(settingsElement);

			foreach (DesktopWindowSetting setting in desktopWindowSettings)
			{
				if (!setting.Bad)
				{
					XmlElement settingElement = document.CreateElement("desktop-window-setting");
					setting.UpdateXmlElement(settingElement);
					settingsElement.AppendChild(settingElement);
				}
			}

			DesktopWindowSettingsXml = document;
			Save();
		}

		#endregion

		#region Public Methods

		public bool GetWindowState(string windowName, out Rectangle windowRegion, out FormWindowState windowState)
		{
			DesktopWindowState existingState = GetDesktopWindowSetting()[windowName];
			if (existingState == null)
			{
				windowRegion = Rectangle.Empty;
				windowState = FormWindowState.Normal;
				return false;
			}

			windowRegion = existingState.WindowRegion;
			windowState = existingState.WindowState;
			return true;
		}

		public void SaveWindowState(string windowName, Rectangle windowRegion, FormWindowState windowState)
		{
			List<DesktopWindowSetting> settings = GetDesktopWindowSettings();
			DesktopWindowSetting currentSetting = DesktopWindowSetting.Get();

			int existingIndex = settings.IndexOf(currentSetting);
			if (existingIndex >= 0)
				currentSetting = settings[existingIndex];
			else	
				settings.Add(currentSetting);
			
			currentSetting.Update(new DesktopWindowState(windowName, windowRegion, windowState));

			SaveDesktopWindowSettings(settings);
		}

		#endregion
	}
}
