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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.ImageViewer.Services.Configuration;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Services.Configuration
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomPublishingConfigurationComponent"/>.
	/// </summary>
	[ExtensionPoint]
	public sealed class DicomPublishingConfigurationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomPublishingConfigurationComponent class.
	/// </summary>
	[AssociateView(typeof(DicomPublishingConfigurationComponentViewExtensionPoint))]
	public class DicomPublishingConfigurationComponent : ApplicationComponentContainer, IConfigurationApplicationComponent
	{
		public class NavigatorHost : ApplicationComponentHost
		{
			private readonly DicomPublishingConfigurationComponent _container;

			internal NavigatorHost(DicomPublishingConfigurationComponent container)
				: base(container._aeNavigator)
			{
				_container = container;
			}

			public override DesktopWindow DesktopWindow
			{
				get { return _container.Host.DesktopWindow; }
			}
		}

		private readonly NavigatorHost _navigatorHost;
		private readonly AENavigatorComponent _aeNavigator;
		private List<string> _selectedServerPaths;

		public DicomPublishingConfigurationComponent()
		{
			_aeNavigator = new AENavigatorComponent();

			_aeNavigator.IsReadOnly = true;
			_aeNavigator.ShowCheckBoxes = true;
			_aeNavigator.ShowLocalDataStoreNode = false;
			_aeNavigator.ShowTitlebar = false;
			_aeNavigator.ShowTools = false;

			StringCollection paths = DicomPublishingSettings.Default.DefaultServerPaths ?? new StringCollection();
			_selectedServerPaths = new List<string>();

			foreach (string path in paths)
			{
				Server server = _aeNavigator.ServerTree.FindServer(path);
				if (server != null && !_selectedServerPaths.Contains(path))
				{
					_selectedServerPaths.Add(path);
					server.IsChecked = true;
				}
			}

			_aeNavigator.ServerTree.ServerTreeUpdated += OnServerTreeUpdated;
			_navigatorHost = new NavigatorHost(this);
		}

		private void OnServerTreeUpdated(object sender, EventArgs e)
		{
			List<IServerTreeNode> selectedServers = _aeNavigator.ServerTree.FindCheckedServers();

			List<string> selectedPaths = CollectionUtils.Map<IServerTreeNode, string>(selectedServers,
												delegate(IServerTreeNode node) { return node.Path; });

			if (!CollectionUtils.Equal<string>(_selectedServerPaths, selectedPaths, false))
			{
				_selectedServerPaths = selectedPaths;
				Modified = true;
			}
		}

		public NavigatorHost AENavigatorHost
		{
			get { return _navigatorHost; }	
		}

		/// <summary>
		/// Called by the host to initialize the application component.
		/// </summary>
		public override void Start()
		{
			base.Start();
			_navigatorHost.StartComponent();
		}

		/// <summary>
		/// Called by the host when the application component is being terminated.
		/// </summary>
		public override void Stop()
		{
			base.Stop();
			_navigatorHost.StopComponent();
		}

		public void Save()
		{
			DicomPublishingSettings.Default.DefaultServerPaths = new StringCollection();
			DicomPublishingSettings.Default.DefaultServerPaths.AddRange(_selectedServerPaths.ToArray());
			DicomPublishingSettings.Default.Save();
		}

		public override IEnumerable<IApplicationComponent> ContainedComponents
		{
			get { yield return _aeNavigator; }
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