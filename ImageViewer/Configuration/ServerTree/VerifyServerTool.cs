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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Dicom.Network.Scu;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ButtonAction("activate", "servertree-toolbar/ToolbarVerify", "VerifyServer")]
	[MenuAction("activate", "servertree-contextmenu/MenuVerify", "VerifyServer")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]
	[Tooltip("activate", "TooltipVerify")]
	[IconSet("activate", IconScheme.Colour, "Icons.VerifyServerToolSmall.png", "Icons.VerifyServerToolMedium.png", "Icons.VerifyServerToolLarge.png")]
	[ExtensionOf(typeof(ServerTreeToolExtensionPoint))]
	public class VerifyServerTool : ServerTreeTool
	{
		public VerifyServerTool()
		{
		}

		private bool NoServersSelected()
		{
			return this.Context.SelectedServers == null || this.Context.SelectedServers.Servers == null || this.Context.SelectedServers.Servers.Count == 0;
		}

		private void VerifyServer()
		{
			BlockingOperation.Run(this.InternalVerifyServer);
		}

		private void InternalVerifyServer()
		{
			if (this.NoServersSelected())
			{
				//should never get here because the verify button should be disabled.
				this.Context.DesktopWindow.ShowMessageBox(SR.MessageNoServersSelected, MessageBoxActions.Ok);
				return;
			}

			string myAE = ImageViewer.Services.ServerTree.ServerTree.GetClientAETitle();

			StringBuilder msgText = new StringBuilder();
			msgText.AppendFormat(SR.MessageCEchoVerificationPrefix + "\r\n\r\n");
			foreach (Server server in this.Context.SelectedServers.Servers)
			{
				using (VerificationScu scu = new VerificationScu())
				{
					VerificationResult result = scu.Verify(myAE, server.AETitle, server.Host, server.Port);
					if (result == VerificationResult.Success)
						msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultSuccess + "\r\n", server.Path);
					else
						msgText.AppendFormat(SR.MessageCEchoVerificationSingleServerResultFail + "\r\n", server.Path);
				}
			}

			msgText.AppendFormat("\r\n");
			this.Context.DesktopWindow.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
		}

		protected override void OnSelectedServerChanged(object sender, EventArgs e)
		{
			this.Enabled = !this.Context.SelectedServers.IsLocalDatastore && !this.NoServersSelected();                 
		}
	}
}