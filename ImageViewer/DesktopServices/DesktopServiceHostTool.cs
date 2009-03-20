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
