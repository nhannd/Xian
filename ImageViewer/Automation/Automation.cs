using System;
using System.Windows.Forms;
using ClearCanvas.Common;
//using ClearCanvas.Workstation.View;

namespace ClearCanvas.Workstation.Automation
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class Automation : MarshalByRefObject
	{
		public Automation()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		public void MaximizeWindow()
		{
			//UIPlugin plugin = (UIPlugin) Platform.GetPlugin("ClearCanvas.Workstation.View");
			//plugin.MainForm.WindowState = FormWindowState.Maximized;
			Platform.Log("testing");
		}
	}
}
