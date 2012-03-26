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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
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

		private readonly ServerTreeComponentHost _serverTreeHost;
		private readonly ServerTreeComponent _serverTreeComponent;
		private readonly ReadOnlyCollection<string> _readOnlySelectedServerPaths;
		private List<string> _selectedServerPaths;
		private string _description;

		protected ServerTreeConfigurationComponent(string description)
		{
			_description = description ?? "";

			_serverTreeComponent = new ServerTreeComponent();

			_serverTreeComponent.IsReadOnly = true;
			_serverTreeComponent.ShowCheckBoxes = true;
			_serverTreeComponent.ShowLocalDataStoreNode = false;
			_serverTreeComponent.ShowTitlebar = false;
			_serverTreeComponent.ShowTools = false;

			StringCollection paths = DefaultServerSettings.Default.DefaultServerPaths ?? new StringCollection();
			_selectedServerPaths = new List<string>();
			_readOnlySelectedServerPaths = new ReadOnlyCollection<string>(_selectedServerPaths);

			foreach (string path in paths)
			{
				var server = _serverTreeComponent.ServerTree.FindServer(path);
				if (server != null && !_selectedServerPaths.Contains(path))
				{
					_selectedServerPaths.Add(path);
					server.IsChecked = true;
				}
			}

			_serverTreeComponent.ServerTree.ServerTreeUpdated += OnServerTreeUpdated;
			_serverTreeHost = new ServerTreeComponentHost(this);
		}

		private void OnServerTreeUpdated(object sender, EventArgs e)
		{
			var checkedServers = _serverTreeComponent.ServerTree.RootServerGroup.GetCheckedServers(true);

            List<string> selectedPaths = CollectionUtils.Map(checkedServers,
												delegate(IServerTreeNode node) { return node.Path; });

			if (!CollectionUtils.Equal<string>(_selectedServerPaths, selectedPaths, false))
			{
				_selectedServerPaths.Clear();
				_selectedServerPaths.AddRange(selectedPaths);
				Modified = true;
			}
		}

		protected ReadOnlyCollection<string> SelectedServerPaths
		{
			get { return _readOnlySelectedServerPaths; }
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

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
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