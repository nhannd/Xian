using System;
using System.Windows.Forms;
using System.Diagnostics;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Help
{
    [MenuAction("showAbout", "global-menus/MenuHelp/MenuAbout")]
	[ClickHandler("showAbout", "ShowAbout")]
	[GroupHint("showAbout", "Application.Help.About")]

    [MenuAction("showWebsite", "global-menus/MenuHelp/MenuWebsite")]
    [ClickHandler("showWebsite", "ShowWebsite")]
	[GroupHint("showWebsite", "Application.Help.Website")]

	[MenuAction("showUsersGuide", "global-menus/MenuHelp/MenuUsersGuide")]
	[ClickHandler("showUsersGuide", "ShowUsersGuide")]
	[GroupHint("showUsersGuide", "Application.Help.UsersGuide")]

	[MenuAction("showLicense", "global-menus/MenuHelp/MenuLicense")]
	[ClickHandler("showLicense", "ShowLicense")]
	[GroupHint("showLicense", "Application.Help.License")]

	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class HelpTool : Tool<IDesktopToolContext>
	{
		public HelpTool()
		{
		}

        public void ShowAbout()
		{
			AboutForm aboutForm = new AboutForm();
			aboutForm.ShowDialog();
		}

		public void ShowWebsite()
		{
			Process.Start("http://www.clearcanvas.ca");
		}

		public void ShowUsersGuide()
		{
			Process.Start("https://mirror2.cvsdude.com/trac/clearcanvas/source/wiki/Users");
		}

		public void ShowLicense()
		{
			string licensePath = String.Format(
				"{0}{1}{2}",
				Platform.InstallDirectory,
				System.IO.Path.DirectorySeparatorChar,
				"License.rtf");

			try
			{
				Process.Start(licensePath);
			}
			catch
			{
				Platform.ShowMessageBox(SR.LicenseNotFound);
			}
		}
	}
}
