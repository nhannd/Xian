using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Services.DicomServer;
using ClearCanvas.ImageViewer.Configuration;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [ExtensionPoint()]
    public class AENavigatorToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    [ExtensionPoint()]
    public class AENavigatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    public interface IAENavigatorToolContext : IToolContext
    {
		IDesktopWindow DesktopWindow { get; }
		ClickHandlerDelegate DefaultActionHandler { get; set; }
				
		ServerTree ServerTree { get; }

		AEServerGroup SelectedServers { get; }
		event EventHandler SelectedServerChanged;
        
        int UpdateType { get; set; }
    }

    [AssociateView(typeof(AENavigatorComponentViewExtensionPoint))]
	public class AENavigatorComponent : ApplicationComponent
	{
        public class AENavigatorToolContext : ToolContext, IAENavigatorToolContext
        {
            AENavigatorComponent _component;

            public AENavigatorToolContext(AENavigatorComponent component)
            {
                Platform.CheckForNullReference(component, "component");
                _component = component; 
            }

            #region IAENavigatorToolContext Members

            public ServerTree ServerTree
            {
                get { return _component._serverTree; }
            }

			public AEServerGroup SelectedServers
			{
				get { return _component.SelectedServers; }
			}

            public event EventHandler SelectedServerChanged
            {
                add { _component.SelectedServerChanged += value; }
                remove { _component.SelectedServerChanged -= value; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            public int UpdateType
            {
                get { return _component.UpdateType; }
                set { _component.UpdateType = value; }
            }

            public ClickHandlerDelegate DefaultActionHandler
            {
                get { return _component._defaultActionHandler; }
                set { _component._defaultActionHandler = value; }
            }

            #endregion
        }

        #region Fields

        private ServerTree _serverTree;
        private event EventHandler _selectedServerChanged;
        private AEServerGroup _selectedServers;
        private int _updateType;
        private ToolSet _toolSet;
        private ActionModelRoot _toolbarModel;
        private ActionModelRoot _contextMenuModel;
        private ClickHandlerDelegate _defaultActionHandler;
		private bool _showTools;
		private bool _showTitlebar;
		private bool _showLocalDataStoreNode;

		public ServerTree ServerTree
        {
            get { return _serverTree; }
            set { _serverTree = value; }
        }

        public AEServerGroup SelectedServers
        {
            get { return _selectedServers; }
            set { _selectedServers = value; }
        }

        public int UpdateType
        {
            get { return _updateType; }
            set { _updateType = value; }
        }

        public ActionModelRoot ToolbarModel
        {
            get { return _toolbarModel; }
            set { _toolbarModel = value; }
        }

        public ActionModelRoot ContextMenuModel
        {
            get { return _contextMenuModel; }
            set { _contextMenuModel = value; }
        }

		public bool ShowTools
		{
			get { return _showTools; }
		}

		public bool ShowTitlebar
		{
			get { return _showTitlebar; }
		}

		public bool ShowLocalDataStoreNode
		{
			get { return _showLocalDataStoreNode; }
		}

        #endregion

        public AENavigatorComponent() : this(true, true, true)
        {
        }

		public AENavigatorComponent(bool showTools, bool showTitlebar, bool showLocalDataStoreNode)
		{
			_showTools = showTools;
			_showTitlebar = showTitlebar;
			_showLocalDataStoreNode = showLocalDataStoreNode;

			_selectedServers = new AEServerGroup();
			_serverTree = new ServerTree();
			_serverTree.RootNode.LocalDataStoreNode.DicomServerConfigurationProvider = DicomServerConfigurationHelper.GetDicomServerConfigurationProvider();

			if (_serverTree.CurrentNode != null && _serverTree.CurrentNode.IsServer || _serverTree.CurrentNode.IsLocalDataStore)
			{
				_selectedServers.Servers.Add(_serverTree.CurrentNode);
				_selectedServers.Name = _serverTree.CurrentNode.Name;
				_selectedServers.GroupID = _serverTree.CurrentNode.ParentPath + "/" + _selectedServers.Name;
			}
		}

		public void SetSelection(IServerTreeNode dataNode)
        {
			if (dataNode.IsServer || dataNode.IsLocalDataStore)
            {
				_selectedServers = new AEServerGroup();
				_selectedServers.Servers.Add(dataNode);
				_selectedServers.Name = dataNode.Name;
				_selectedServers.GroupID = dataNode.Path;
				_serverTree.CurrentNode = dataNode;
				FireSelectedServerChangedEvent();
            }
            else if (dataNode.IsServerGroup)
            {
                _selectedServers = new AEServerGroup();
                _selectedServers.Servers = _serverTree.FindChildServers(dataNode as ServerGroup);
                _selectedServers.GroupID = dataNode.Path;
                _selectedServers.Name = dataNode.Name;
                _serverTree.CurrentNode = dataNode;
                FireSelectedServerChangedEvent();
            }

        }

        public bool NodeMoved(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
        {
            if (destinationNode.IsServer || isMovingInvalid(destinationNode as ServerGroup, movingDataNode))
                return false;

            if (movingDataNode.IsServer)
            {
                Server movingServer = movingDataNode as Server;
                _serverTree.CurrentNode = movingDataNode;
                _serverTree.DeleteDicomServer();

                movingServer.ChangeParentPath(destinationNode.Path);
                (destinationNode as ServerGroup).AddChild(movingDataNode);
                SetSelection(movingDataNode);
            }
            else if (movingDataNode.IsServerGroup)
            {
                ServerGroup movingGroup = movingDataNode as ServerGroup;
                _serverTree.CurrentNode = movingGroup;
                _serverTree.DeleteServerGroup();

                movingGroup.ChangeParentPath(destinationNode.Path);
                (destinationNode as ServerGroup).AddChild(movingGroup);
                SetSelection(movingGroup);
            }
            _serverTree.SaveDicomServers();
            return true;
        }

        public event EventHandler SelectedServerChanged
        {
            add { _selectedServerChanged += value; }
            remove { _selectedServerChanged -= value; }
        }

        private bool isMovingInvalid(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
        {
            if (movingDataNode.Name.Equals(_serverTree.MyServersTitle) || movingDataNode.Name.Equals(_serverTree.MyDatastoreTitle) || movingDataNode.Path.Equals(destinationNode.Path))
                return true;

            foreach (Server server in (destinationNode as ServerGroup).ChildServers)
            {
                if (server.Name.Equals(movingDataNode.Name))
                    return true;
            }

            if (!movingDataNode.IsServer && destinationNode.Path.StartsWith(movingDataNode.Path))
                return true;

            return false;
        }

        private void FireSelectedServerChangedEvent()
        {
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            _toolSet = new ToolSet(new AENavigatorToolExtensionPoint(), new AENavigatorToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomaenavigator-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomaenavigator-contextmenu", _toolSet.Actions);
		}

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

        public void NodeDoubleClick()
        {
            // according to the framework architecture, the default action handler
            // for this component is set up by the ServerEditTool
            // however, since the tool is used for both Server and ServerGroup
            // and we want to retain the behaviour of expanding the tree node when
            // a ServerGroup is d-clicked, we only want the edit tool invoked if
            // the node is a Server
            if (!_serverTree.CurrentNode.IsLocalDataStore && 
                !_serverTree.CurrentNode.IsServerGroup && 
                null != _defaultActionHandler &&
				_showTools)
                _defaultActionHandler();
        }
    }

    public enum ServerUpdateType
    {
        None = 0,
		Add,
        Edit,
        Delete
    }
}
