#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.EnterpriseDesktop
{
	[MenuAction("changePassword", "global-menus/MenuTools/MenuChangePassword", "ChangePassword")]
	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class SessionManagerTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		public SessionManagerTool()
		{
		}

		public void ChangePassword()
		{
			if (SessionManager.ChangePassword())
			{
				this.Context.DesktopWindow.ShowMessageBox(SR.MessagePasswordChanged, MessageBoxActions.Ok);
			}
		}
	}
}
