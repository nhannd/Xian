#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.ImageViewer.Configuration.ServerTree
{
	[ExtensionPoint]
	public sealed class ServerTreeConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(ServerTreeConfigurationComponentViewExtensionPoint))]
	public abstract class ServerTreeConfigurationComponent : ConfigurationApplicationComponentContainer
	{
		public class ServerTreeComponentHost : ApplicationComponentHost
		{
			private readonly ServerTreeConfigurationComponent _container;

			internal ServerTreeComponentHost(ServerTreeConfigurationComponent container)
				: base(container._serverTreeComponent)
			{
				_container = container;
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _container.Host.DesktopWindow; }
			}
		}

		private ServerTreeComponentHost _serverTreeHost;
		private ServerTreeComponent _serverTreeComponent;
        private DicomServiceNodeList _checkedServers;
		
        private string _description;

        protected ServerTreeConfigurationComponent(string description, DicomServiceNodeList checkedServers)
		{
			_description = description ?? "";
            _checkedServers = checkedServers;
		}

	    protected DicomServiceNodeList CheckedServers
	    {
	        get { return _checkedServers ?? (_checkedServers = new DicomServiceNodeList()); }
            private set { _checkedServers = value; }
	    }

	    public string Description
		{
            get { return _description; }
	        set
			{
				if (_description == value)
					return;

				_description = value;
				NotifyPropertyChanged("Description");
			}
		}

		public ServerTreeComponentHost ServerTreeHost
		{
			get { return _serverTreeHost; }	
		}

        private void InitializeCheckedServers()
        {
            var all = _serverTreeComponent.ServerTree.RootServerGroup.GetAllServers();
            foreach (var server in all)
            {
                var @checked = CheckedServers.FirstOrDefault(s => s.Name == server.Name);
                server.IsChecked = @checked != null;
            }
        }

        private void OnServerTreeUpdated(object sender, EventArgs e)
        {
            var checkedServers = _serverTreeComponent.ServerTree.RootServerGroup.GetCheckedServers(true);
            CheckedServers = new DicomServiceNodeList(checkedServers.SelectMany(s => s.ToDicomServiceNodes()));
            Modified = true;
        }

	    /// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
            _serverTreeComponent = new ServerTreeComponent
            {
                IsReadOnly = true,
                ShowCheckBoxes = true,
                ShowLocalServerNode = false,
                ShowTitlebar = false,
                ShowTools = false
            };

            InitializeCheckedServers();

            _serverTreeComponent.ServerTree.ServerTreeUpdated += OnServerTreeUpdated;
            _serverTreeHost = new ServerTreeComponentHost(this);
            
            base.Start();
			_serverTreeHost.StartComponent();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			_serverTreeHost.StopComponent();
		}

		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get { yield return _serverTreeComponent; }
		}

		public override IEnumerable<IApplicationComponent> VisibleComponents
		{
			get { return this.ContainedComponents; }
		}

		public override void EnsureVisible(IApplicationComponent component)
		{
			if (!this.IsStarted)
				throw new InvalidOperationException("The container was never started.");

			// nothing to do, since the hosted components are started by default
		}

		public override void EnsureStarted(IApplicationComponent component)
		{
			if (!this.IsStarted)
				throw new InvalidOperationException("The container was never started.");

			// nothing to do, since the hosted components are visible by default
		}
	}
}