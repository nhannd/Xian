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
using System.Security.Policy;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Common;
using ClearCanvas.ImageViewer.Common.StudyManagement;
using ClearCanvas.ImageViewer.Configuration.ServerTree;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	internal class DicomExplorerComponent : SplitComponentContainer
	{
		private static readonly object _syncLock = new object();
		private static readonly List<DicomExplorerComponent> _activeComponents = new List<DicomExplorerComponent>();

		private ServerTreeComponent _serverTreeComponent;
		private IStudyBrowserComponent _studyBrowserComponent;
		private ISearchPanelComponent _searchPanelComponent;

		private DicomExplorerComponent(SplitPane pane1, SplitPane pane2)
			: base(pane1, pane2, Desktop.SplitOrientation.Horizontal)
		{
		}

		public ServerTreeComponent ServerTreeComponent
		{
			get { return _serverTreeComponent; }
		}

		public IStudyBrowserComponent StudyBrowserComponent
		{
			get { return _studyBrowserComponent; }
		}

		public ISearchPanelComponent SearchPanelComponent
		{
			get { return _searchPanelComponent; }
		}

		public override void Start()
		{
			base.Start();

			lock (_syncLock)
			{
				_activeComponents.Add(this);
			}

			_searchPanelComponent.SearchRequested += OnSearchPanelComponentSearchRequested;
			_searchPanelComponent.SearchCancelled += OnSearchPanelComponentSearchCancelled;
			_studyBrowserComponent.SearchStarted += OnStudyBrowserComponentSearchStarted;
			_studyBrowserComponent.SearchEnded += OnStudyBrowserComponentSearchCompleted;

			// if initially selected server is local, begin an initial dicom query
			if (_serverTreeComponent.ShowLocalServerNode && _serverTreeComponent.SelectedServers.IsLocalServer)
			{
				try
				{
                    var queryParams = new QueryParameters();
					var queryParamList = new List<QueryParameters> {queryParams};
					_studyBrowserComponent.Search(queryParamList);
				}
				catch (PolicyException)
				{
					//TODO: ignore this on startup or show message?
				}
				catch (Exception e)
				{
					ExceptionHandler.Report(e, this.Host.DesktopWindow);
				}
			}
		}

		public override void Stop()
		{
			_searchPanelComponent.SearchRequested -= OnSearchPanelComponentSearchRequested;
			_searchPanelComponent.SearchCancelled -= OnSearchPanelComponentSearchCancelled;
			_studyBrowserComponent.SearchStarted -= OnStudyBrowserComponentSearchStarted;
			_studyBrowserComponent.SearchEnded -= OnStudyBrowserComponentSearchCompleted;

			lock (_syncLock)
			{
				_activeComponents.Remove(this);
			}

			base.Stop();
		}

		public static List<DicomExplorerComponent> GetActiveComponents()
		{
			lock (_syncLock)
			{
				return new List<DicomExplorerComponent>(_activeComponents);
			}
		}

		public static DicomExplorerComponent Create()
		{
			ServerTreeComponent serverTreeComponent = new ServerTreeComponent();
			serverTreeComponent.ShowLocalServerNode = StudyStore.IsSupported;

			bool hasEditPermission = PermissionsHelper.IsInRole(AuthorityTokens.Configuration.MyServers);
			serverTreeComponent.IsReadOnly = !hasEditPermission;

			var studyBrowserComponent = CreateComponentFromExtensionPoint<StudyBrowserComponentExtensionPoint, IStudyBrowserComponent>()
				?? new StudyBrowserComponent();

			serverTreeComponent.SelectedServerChanged +=
				delegate { studyBrowserComponent.SelectedServerGroup = serverTreeComponent.SelectedServers; };

			var searchPanelComponent = CreateComponentFromExtensionPoint<SearchPanelComponentExtensionPoint, ISearchPanelComponent>()
				?? new SearchPanelComponent();
			SelectDefaultServerNode(serverTreeComponent);

			SplitPane leftPane = new SplitPane(SR.TitleServerTreePane, serverTreeComponent, 0.25f);
			SplitPane rightPane = new SplitPane(SR.TitleStudyBrowserPane, studyBrowserComponent, 0.75f);

			SplitComponentContainer bottomContainer =
				new SplitComponentContainer(
				leftPane,
				rightPane,
				SplitOrientation.Vertical);

			SplitPane topPane = new SplitPane(SR.TitleSearchPanelPane, searchPanelComponent, true);
			SplitPane bottomPane = new SplitPane(SR.TitleStudyNavigatorPane, bottomContainer, false);

			DicomExplorerComponent component = new DicomExplorerComponent(topPane, bottomPane);
			component._studyBrowserComponent = studyBrowserComponent;
			component._searchPanelComponent = searchPanelComponent;
			component._serverTreeComponent = serverTreeComponent;
			return component;
		}

		private void OnStudyBrowserComponentSearchStarted(object sender, EventArgs e)
		{
			_searchPanelComponent.SearchInProgress = true;
			_serverTreeComponent.IsEnabled = false;
		}

		private void OnStudyBrowserComponentSearchCompleted(object sender, EventArgs e)
		{
			_searchPanelComponent.SearchInProgress = false;
			_serverTreeComponent.IsEnabled = true;
		}

		private void OnSearchPanelComponentSearchRequested(object sender, SearchRequestedEventArgs e)
		{
			try
			{
				_studyBrowserComponent.Search(new List<QueryParameters>(e.QueryParametersList));
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Host.DesktopWindow);
			}
		}

		private void OnSearchPanelComponentSearchCancelled(object sender, EventArgs e)
		{
			try
			{
				_studyBrowserComponent.CancelSearch();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, this.Host.DesktopWindow);
			}
		}

		internal void SelectDefaultServers()
		{
			SelectDefaultServers(_serverTreeComponent);
		}

		private static void SelectDefaultServerNode(ServerTreeComponent serverTreeComponent)
		{
			if (serverTreeComponent.ShowLocalServerNode &&
				!DicomExplorerConfigurationSettings.Default.SelectDefaultServerOnStartup)
			{
				serverTreeComponent.SetSelection(serverTreeComponent.ServerTree.LocalServer);
			}
			else
			{
				SelectDefaultServers(serverTreeComponent);
			}
		}

	    // TODO (Marmot): take this away?
		private static void SelectDefaultServers(ServerTreeComponent serverTreeComponent)
		{
            ServerTree serverTree = serverTreeComponent.ServerTree;

            var defaultServers = DefaultServers.GetAll();
            CheckDefaultServers(serverTree, defaultServers);
            IServerTreeNode initialSelection = GetFirstDefaultServerOrGroup(serverTree.RootServerGroup);
            UncheckAllServers(serverTree);

            if (initialSelection == null)
            {
                if (serverTreeComponent.ShowLocalServerNode)
                    initialSelection = serverTreeComponent.ServerTree.LocalServer;
                else
                    initialSelection = serverTreeComponent.ServerTree.RootServerGroup;
            }

            serverTreeComponent.SetSelection(initialSelection);
		}

        private static IServerTreeNode GetFirstDefaultServerOrGroup(IServerTreeGroup serverGroup)
        {
            if (serverGroup.IsEntireGroupChecked())
                return serverGroup;

            //consider groups and servers at this level
            foreach (IServerTreeGroup group in serverGroup.ChildGroups)
            {
                if (group.IsEntireGroupChecked())
                    return group;
            }

            foreach (IServerTreeDicomServer server in serverGroup.Servers)
            {
                if (server.IsChecked)
                    return server;
            }

            //repeat for children of the groups at this level
            foreach (IServerTreeGroup group in serverGroup.ChildGroups)
            {
                IServerTreeNode defaultServerOrGroup = GetFirstDefaultServerOrGroup(group);
                if (defaultServerOrGroup != null)
                    return defaultServerOrGroup;
            }

            return null;
        }

        private static void CheckDefaultServers(ServerTree serverTree, List<IDicomServiceNode> defaultServers)
        {
            foreach (IServerTreeDicomServer server in serverTree.FindChildServers())
            {
                var treeServer = server;
                if (defaultServers.Any(s => s.Name == treeServer.Name))
                    server.IsChecked = true;
            }
        }

        private static void UncheckAllServers(ServerTree serverTree)
        {
            foreach (IServerTreeDicomServer server in serverTree.FindChildServers())
                server.IsChecked = false;
        }

		private static TComponent CreateComponentFromExtensionPoint<TExtensionPoint, TComponent>()
			where TExtensionPoint : IExtensionPoint, new()
			where TComponent : class, IApplicationComponent
		{
			try
			{
				var xp = new TExtensionPoint();
				return (TComponent)xp.CreateExtension();
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}
