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

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ClearCanvas.Common.Configuration;
using System.Xml;
using System.ComponentModel;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
	[SettingsGroupDescription("Stores window positions for the application, so they can be restored the next time it runs.")]
	[SettingsProvider(typeof(StandardSettingsProvider))]
	internal sealed partial class DesktopViewSettings
	{
		#region FloatingShelfState class
		
		private class FloatingShelfState : IEquatable<FloatingShelfState>
		{
			public FloatingShelfState(string desktopWindowName, string name, Point displayLocation)
			{
				DesktopWindowName = desktopWindowName ?? "";
				Name = name ?? "";
				DisplayLocation = displayLocation;
			}

			public readonly string DesktopWindowName;
			public readonly string Name;
			public readonly Point DisplayLocation;

			public static FloatingShelfState FromXmlElement(XmlElement element)
			{
				string desktopWindowName = element.GetAttribute("desktop-window-name");
				string name = element.GetAttribute("shelf-name");

				TypeConverter converter = TypeDescriptor.GetConverter(typeof (Point));
				Point displayLocation = (Point)converter.ConvertFromString(null, CultureInfo.InvariantCulture, element.GetAttribute("display-location"));

				return new FloatingShelfState(desktopWindowName, name, displayLocation);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				element.SetAttribute("desktop-window-name", DesktopWindowName);
				element.SetAttribute("shelf-name", Name);

				TypeConverter converter = TypeDescriptor.GetConverter(typeof (Point));
				element.SetAttribute("display-location", converter.ConvertToString(null, CultureInfo.InvariantCulture, this.DisplayLocation));
			}

			public override bool Equals(object obj)
			{
				if (obj == this)
					return true;
				if (obj is FloatingShelfState)
					return Equals((FloatingShelfState)obj);

				return false;
			}

			public override int  GetHashCode()
			{
 				 return base.GetHashCode();
			}
			
			#region IEquatable<FloatingShelfState> Members

			public bool  Equals(FloatingShelfState other)
			{
				return other != null && this.DesktopWindowName == other.DesktopWindowName && this.Name == other.Name;
			}

			#endregion
		}

		#endregion

		#region DesktopWindowState class 

		private class DesktopWindowState : IEquatable<DesktopWindowState>
		{
			public DesktopWindowState(string windowName, Rectangle windowRegion, FormWindowState windowState)
			{
				Name = windowName ?? "";
				WindowRegion = windowRegion;
				WindowState = windowState;
			}

			public readonly string Name;
			public readonly Rectangle WindowRegion;
			public readonly FormWindowState WindowState;
			
			public static DesktopWindowState FromXmlElement(XmlElement element)
			{
				string name = element.GetAttribute("name");

				TypeConverter converter = TypeDescriptor.GetConverter(typeof (Rectangle));
				Rectangle windowRegion = (Rectangle)converter.ConvertFromString(null, CultureInfo.InvariantCulture, element.GetAttribute("region"));

				converter = TypeDescriptor.GetConverter(typeof(FormWindowState));
				FormWindowState windowState = (FormWindowState)converter.ConvertFromString(null, CultureInfo.InvariantCulture, element.GetAttribute("state"));

				return new DesktopWindowState(name, windowRegion, windowState);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				element.SetAttribute("name", Name ?? "");

				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				element.SetAttribute("region", converter.ConvertToString(null, CultureInfo.InvariantCulture, WindowRegion));

				converter = TypeDescriptor.GetConverter(typeof(FormWindowState));
				element.SetAttribute("state", converter.ConvertToString(null, CultureInfo.InvariantCulture, WindowState));
			}

			public override int  GetHashCode()
			{
 				 return base.GetHashCode();
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
				return other != null && other.Name.Equals(Name);
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
				Rectangle region = (Rectangle)converter.ConvertFromString(null, CultureInfo.InvariantCulture, element.GetAttribute("region"));

				return new ScreenConfiguration(region);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				element.SetAttribute("region", converter.ConvertToString(null, CultureInfo.InvariantCulture, ScreenRegion));
			}

			public static ScreenConfiguration[] Get()
			{
				List<ScreenConfiguration> configurations = new List<ScreenConfiguration>();

				foreach (Screen screen in Screen.AllScreens)
					configurations.Add(new ScreenConfiguration(screen.Bounds));

				return configurations.ToArray();
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
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
				return other != null && other.ScreenRegion.Equals(ScreenRegion);
			}

			#endregion
		}

		#endregion

		#region DesktopWindowSetting class

		private class DesktopWindowSetting : IEquatable<DesktopWindowSetting>
		{
			private readonly List<DesktopWindowState> _desktopWindowStates;
			private readonly List<FloatingShelfState> _floatingShelfStates;

			private DesktopWindowSetting(Rectangle virtualScreenRectangle, ScreenConfiguration[] screenConfigurations)
				: this(virtualScreenRectangle, screenConfigurations, new List<DesktopWindowState>(), new List<FloatingShelfState>())
			{
			}

			private DesktopWindowSetting(
				Rectangle virtualScreenRectangle, 
				ScreenConfiguration[] screenConfigurations, 
				List<DesktopWindowState> desktopWindowStates, 
				List<FloatingShelfState> floatingShelfStates)
			{
				VirtualScreenRectangle = virtualScreenRectangle;
				ScreenConfigurations = screenConfigurations;
				_desktopWindowStates = desktopWindowStates;
				_floatingShelfStates = floatingShelfStates;
			}
			
			public readonly Rectangle VirtualScreenRectangle;
			public readonly ScreenConfiguration[] ScreenConfigurations;

			public static DesktopWindowSetting FromXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof (Rectangle));
				Rectangle virtualScreenRectangle = (Rectangle)converter.ConvertFromString(null, CultureInfo.InvariantCulture, element.GetAttribute("virtual-screen-rectangle"));

				List<ScreenConfiguration> screenConfigurations = new List<ScreenConfiguration>();
				foreach (XmlElement screenConfigurationElement in element.SelectNodes("screen-configurations/screen-configuration"))
					screenConfigurations.Add(ScreenConfiguration.FromXmlElement(screenConfigurationElement));

				List<DesktopWindowState> desktopWindowStates = new List<DesktopWindowState>();
				foreach (XmlElement desktopWindowStateElement in element.SelectNodes("desktop-window-states/desktop-window-state"))
					desktopWindowStates.Add(DesktopWindowState.FromXmlElement(desktopWindowStateElement));

				List<FloatingShelfState> floatingShelfStates = new List<FloatingShelfState>();
				foreach (XmlElement floatingShelfStateElement in element.SelectNodes("floating-shelf-states/floating-shelf-state"))
					floatingShelfStates.Add(FloatingShelfState.FromXmlElement(floatingShelfStateElement));

				return new DesktopWindowSetting(virtualScreenRectangle, screenConfigurations.ToArray(), desktopWindowStates, floatingShelfStates);
			}

			public void UpdateXmlElement(XmlElement element)
			{
				TypeConverter converter = TypeDescriptor.GetConverter(typeof(Rectangle));
				element.SetAttribute("virtual-screen-rectangle", converter.ConvertToString(null, CultureInfo.InvariantCulture, VirtualScreenRectangle));

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

				foreach (DesktopWindowState desktopWindowState in _desktopWindowStates)
				{
					XmlElement desktopWindowStateElement = element.OwnerDocument.CreateElement("desktop-window-state");
					desktopWindowState.UpdateXmlElement(desktopWindowStateElement);
					desktopWindowStatesElement.AppendChild(desktopWindowStateElement);
				}

				XmlElement floatingShelfStatesElement = element.OwnerDocument.CreateElement("floating-shelf-states");
				element.AppendChild(floatingShelfStatesElement);
				foreach (FloatingShelfState floatingShelfState in _floatingShelfStates)
				{
					XmlElement floatingShelfStateElement = element.OwnerDocument.CreateElement("floating-shelf-state");
					floatingShelfState.UpdateXmlElement(floatingShelfStateElement);
					floatingShelfStatesElement.AppendChild(floatingShelfStateElement);
				}
			}

			public bool Bad
			{
				get { return ScreenConfigurations.Length == 0; }	
			}

			public DesktopWindowState GetDesktopWindowState(string name)
			{
				name = name ?? "";
				return _desktopWindowStates.Find(delegate(DesktopWindowState test) { return test.Name == name; });
			}

			public FloatingShelfState GetFloatingShelfState(string desktopWindowName, string shelfName)
			{
				desktopWindowName = desktopWindowName ?? "";
				shelfName = shelfName ?? "";
				return _floatingShelfStates.Find(delegate(FloatingShelfState test) { return test.DesktopWindowName == desktopWindowName && test.Name == shelfName; });
			}

			public void Update(DesktopWindowState state)
			{
				int existingStateIndex = _desktopWindowStates.IndexOf(state);
				if (existingStateIndex >= 0)
					_desktopWindowStates[existingStateIndex] = state;
				else
					_desktopWindowStates.Add(state);
			}

			public void Update(FloatingShelfState state)
			{
				int existingStateIndex = _floatingShelfStates.IndexOf(state);
				if (existingStateIndex >= 0)
					_floatingShelfStates[existingStateIndex] = state;
				else
					_floatingShelfStates.Add(state);
			}

			public static DesktopWindowSetting Get()
			{
				return new DesktopWindowSetting(SystemInformation.VirtualScreen, ScreenConfiguration.Get());
			}

			public override int GetHashCode()
			{
				return base.GetHashCode();
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
				if (other == null)
					return false;

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
						settings[existingIndex] = setting;
					else 
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

		public bool GetDesktopWindowState(string windowName, out Rectangle windowRegion, out FormWindowState windowState)
		{
			DesktopWindowState existingState = GetDesktopWindowSetting().GetDesktopWindowState(windowName);
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

		public void SaveDesktopWindowState(string windowName, Rectangle windowRegion, FormWindowState windowState)
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

		public bool GetFloatingShelfState(string desktopWindowName, string shelfName, out Point displayLocation)
		{
			FloatingShelfState existingState = GetDesktopWindowSetting().GetFloatingShelfState(desktopWindowName, shelfName);
			if (existingState == null)
			{
				displayLocation = Point.Empty;
				return false;
			}
			else
			{
				displayLocation = existingState.DisplayLocation;
				return true;
			}
		}

		public void SaveFloatingShelfState(string desktopWindowName, string shelfName, Point displayLocation)
		{
			if (String.IsNullOrEmpty(shelfName))
				return;

			List<DesktopWindowSetting> settings = GetDesktopWindowSettings();
			DesktopWindowSetting currentSetting = DesktopWindowSetting.Get();

			int existingIndex = settings.IndexOf(currentSetting);
			if (existingIndex >= 0)
				currentSetting = settings[existingIndex];
			else
				settings.Add(currentSetting);

			currentSetting.Update(new FloatingShelfState(desktopWindowName, shelfName, displayLocation));

			SaveDesktopWindowSettings(settings);
		}

		#endregion
	}
}
