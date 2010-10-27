#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.Remoting;

namespace RemotingClient
{
	//JR: this tool was only used to test a potential .NET remoting solution
	//[MenuAction("apply", "global-menus/Test/Remoting Service", "Apply")]
	[Tooltip("apply", "Place tooltip text here")]
	[IconSet("apply", IconScheme.Colour, "Icons.EchoToolSmall.png", "Icons.EchoToolMedium.png", "Icons.EchoToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]

	[ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
	public class EchoTool : Tool<ClearCanvas.Desktop.IDesktopToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		private IEchoService _echoService;

		/// <summary>
		/// Default constructor.  A no-args constructor is required by the
		/// framework.  Do not remove.
		/// </summary>
		public EchoTool()
		{
			_enabled = true;
		}

		public override void Initialize()
		{
			base.Initialize();

			try
			{
				RemotingConfiguration.Configure("RemotingClient.dll.config", true);
				//RemotingConfiguration.Configure(null, true);

			}
			catch (Exception e)
			{
				this.Context.DesktopWindow.ShowMessageBox(e.ToString(), MessageBoxActions.Ok);
			}
		}

		/// <summary>
		/// Called to determine whether this tool is enabled/disabled in the UI.
		/// </summary>
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

		/// <summary>
		/// Notifies that the Enabled state of this tool has changed.
		/// </summary>
		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

		/// <summary>
		/// Called by the framework when the user clicks the "apply" menu item or toolbar button.
		/// </summary>
		public void Apply()
		{
			try
			{
				_echoService = (IEchoService)Activator.GetObject(typeof(IEchoService), RemotingSettings.Default.RemoteHostUrl);

				this.Context.DesktopWindow.ShowMessageBox(_echoService.Echo("If you see this, remoting is working."),
														  MessageBoxActions.Ok);

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
