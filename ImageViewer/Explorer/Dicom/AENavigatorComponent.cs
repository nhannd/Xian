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
        NewServerTree ServerTree { get; set;}
        event EventHandler SelectedServerChanged;
        IDesktopWindow DesktopWindow { get; }
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

            public NewServerTree ServerTree
            {
                get { return _component._serverTree; }
                set { _component._serverTree = value; }
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

            #endregion
        }

        #region Fields

        private NewServerTree _serverTree;
        private event EventHandler _selectedServerChanged;
        private AEServerGroup _selectedServers;
        private int _updateType;
        private ToolSet _toolSet;
        private ActionModelRoot _toolbarModel;
        private ActionModelRoot _contextMenuModel;

		private static String _myServersTitle = SR.TitleMyServers;
		private static String _myDatastoreTitle = SR.TitleMyDataStore;
        private static String _myServersRoot = "MyServersRoot";
        private static String _myServersXmlFile = "DicomAEServers.xml";

        public NewServerTree ServerTree
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

        public static String MyServersTitle
        {
            get { return AENavigatorComponent._myServersTitle; }
        }

        public static String MyDatastoreTitle
        {
            get { return AENavigatorComponent._myDatastoreTitle; }
        }

        public static String MyServersRoot
        {
            get { return AENavigatorComponent._myServersRoot; }
        }

        public static String MyServersXmlFile
        {
            get { return AENavigatorComponent._myServersXmlFile; }
        }

        #endregion

        public AENavigatorComponent()
        {
            _selectedServers = new AEServerGroup();
            _serverTree = new NewServerTree();

            if (_serverTree.CurrentNode != null && _serverTree.CurrentNode.IsServer || _serverTree.CurrentNode.IsLocalDataStore)
            {
                _selectedServers.Servers.Add(_serverTree.CurrentNode as Server);
                _selectedServers.Name = _serverTree.CurrentNode.Name;
                _selectedServers.GroupID = _serverTree.CurrentNode.ParentPath + "/" + _selectedServers.Name;
            }
        }

        public void SelectChanged(IServerTreeNode dataNode)
        {
            if (dataNode.IsServer)
            {
                SetSelectedServer(dataNode as Server);
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
                SelectChanged(movingDataNode);
            }
            else if (movingDataNode.IsServerGroup)
            {
                ServerGroup movingGroup = movingDataNode as ServerGroup;
                _serverTree.CurrentNode = movingGroup;
                _serverTree.DeleteServerGroup();

                movingGroup.ChangeParentPath(destinationNode.Path);
                (destinationNode as ServerGroup).AddChild(movingGroup);
                SelectChanged(movingGroup);
            }
            _serverTree.SaveDicomServers();
            return true;
        }

        public event EventHandler SelectedServerChanged
        {
            add { _selectedServerChanged += value; }
            remove { _selectedServerChanged -= value; }
        }

        private void SetSelectedServer(Server server)
        {
            _selectedServers = new AEServerGroup();
            _selectedServers.Servers.Add(server);
            _selectedServers.Name = server.Name;
            _selectedServers.GroupID = server.Path;
            _serverTree.CurrentNode = server;
            FireSelectedServerChangedEvent();
        }

        private bool isMovingInvalid(IServerTreeNode destinationNode, IServerTreeNode movingDataNode)
        {
            if (movingDataNode.Name.Equals(_myServersTitle) || movingDataNode.Name.Equals(_myDatastoreTitle) || movingDataNode.Path.Equals(destinationNode.Path))
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

    }

    public enum ServerUpdateType
    {
        Add,
        Edit,
        Delete
    }
}
