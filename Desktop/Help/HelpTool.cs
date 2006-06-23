using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.Help
{
	[MenuAction("activate", "MenuHelp/MenuHelpAbout")]
    [ClickHandler("activate", "Activate")]
	/// <summary>
	/// Summary description for Help.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class HelpTool : Tool
	{
		public HelpTool()
		{
		}

        public void Activate()
		{
			AboutForm aboutForm = new AboutForm();
			aboutForm.ShowDialog();
		}
	}
}
