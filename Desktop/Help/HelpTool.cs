using System;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Common.Application.Tools;
using ClearCanvas.Common.Application.Actions;
using ClearCanvas.Workstation.Model;

namespace ClearCanvas.Workstation.Help
{
	[MenuAction("activate", "MenuHelp/MenuHelpAbout")]
    [ClickHandler("activate", "Activate")]
	/// <summary>
	/// Summary description for Help.
	/// </summary>
    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Common.Application.WorkstationToolExtensionPoint))]
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
