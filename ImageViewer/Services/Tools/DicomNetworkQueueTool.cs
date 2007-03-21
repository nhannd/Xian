using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Services.Tools
{
	[MenuAction("activate", "global-menus/MenuTools/DicomNetworkQueue")]
	//[ButtonAction("activate", "global-toolbars/ToolbarMyTools/DicomNetworkQueue")]
	//[Tooltip("activate", "Place tooltip text here")]
	[IconSet("activate", IconScheme.Colour, "Icons.DicomNetworkQueueSmall.png", "Icons.DicomNetworkQueueMedium.png", "")]
	[ClickHandler("activate", "Activate")]
	[EnabledStateObserver("activate", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class DicomNetworkQueueTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;
		private LocalDataStoreActivityMonitor _monitor;
		private IWorkspace _workspace;

		public DicomNetworkQueueTool()
		{
			_enabled = true;
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public bool Enabled
		{
			get { return _enabled; }
			protected set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		public void Activate()
		{
			if (_workspace != null)
			{
				_workspace.Activate();
			}
			else
			{
				_monitor = new LocalDataStoreActivityMonitor();
				try
				{
					_monitor.Start();
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
					return;
				}

				try
				{ 
					DicomNetworkReceiveQueueApplicationComponent component = new DicomNetworkReceiveQueueApplicationComponent(_monitor);
					_workspace = ApplicationComponent.LaunchAsWorkspace(this.Context.DesktopWindow, component, "Dicom Network Queue",
						delegate(IApplicationComponent c)
						{ 
							_workspace = null;
							try
							{
								_monitor.Stop();
								_monitor = null;
							}
							catch (Exception e)
							{
								ExceptionHandler.Report(e, this.Context.DesktopWindow);
							}
						});
				}
				catch(Exception e)
				{
					ExceptionHandler.Report(e, this.Context.DesktopWindow);
				}
			}
		}
	}
}
