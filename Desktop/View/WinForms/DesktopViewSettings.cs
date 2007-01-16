#if !MONO

using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop.View.WinForms
{
    [SettingsProvider(typeof(ClearCanvas.Common.Configuration.StandardSettingsProvider))]
	public class DesktopViewSettings : ExtensionSettings
	{
		public DesktopViewSettings()
		{
		}

		[UserScopedSettingAttribute()]
		[DefaultSettingValueAttribute("0, 0, 0, 0")]
		public Rectangle WindowRectangle
		{
			get { return (Rectangle)this["WindowRectangle"]; }
			set { this["WindowRectangle"] = value; }
		}

		[UserScopedSettingAttribute()]
		[DefaultSettingValueAttribute("Normal")]
		public FormWindowState WindowState
		{
			get { return (FormWindowState)this["WindowState"]; }
			set { this["WindowState"] = value; }
		}

		[UserScopedSettingAttribute()]
		[DefaultSettingValueAttribute("0, 0, 0, 0")]
		public Rectangle PrimaryScreenRectangle
		{
			get { return (Rectangle)this["PrimaryScreenRectangle"]; }
			set { this["PrimaryScreenRectangle"] = value; }
		}
	}
}

#endif //!MONO