#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.ImageViewer.Common;

namespace ClearCanvas.ImageViewer.DesktopServices
{
	public abstract class DesktopServiceHostTool : Tool<IDesktopToolContext>
	{
		internal static SynchronizationContext HostSynchronizationContext;
		internal static AppDomain HostAppDomain;

		private ServiceHost _host = null;

		protected DesktopServiceHostTool()
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			HostSynchronizationContext = SynchronizationContext.Current;
			HostAppDomain = AppDomain.CurrentDomain;

			StartHost();
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			StopHost();
		}

		private void StartHost()
		{
			try
			{
				object[] permissionAttributes = this.GetType().GetCustomAttributes(typeof (DesktopServiceHostPermissionAttribute), true);
				foreach (DesktopServiceHostPermissionAttribute permissionAttribute in permissionAttributes)
				{
					if (!PermissionsHelper.IsInRoles(permissionAttribute.AuthorityTokens))
					{
						Platform.Log(LogLevel.Warn, "User does not have appropriate permissions to start desktop service ('{0}').", this.GetType().FullName);
						return;
					}
				}

				ServiceHost host = CreateServiceHost();
				host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.None;
				host.Open();
				_host = host;
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Unable to start desktop service; another instance may be running.");
			}
		}

		protected abstract ServiceHost CreateServiceHost();

		private void StopHost()
		{
			try
			{
				if (_host != null)
				{
					_host.Close();
					_host = null;
				}
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failed to stop desktop service.");
			}
		}
	}
}
