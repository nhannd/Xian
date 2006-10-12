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
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	[ExtensionPoint()]
	public class AENavigatorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(AENavigatorComponentViewExtensionPoint))]
	public class AENavigatorComponent : ApplicationComponent
	{
        #region Fields

        private DicomServerTree _dicomServerTree;
        private event EventHandler _selectedServerChanged;
        private AEServerGroup _selectedServers;

        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My Studies";
        private static String _myServersRoot = "MyServersRoot";
        private static String _myServersXmlFile = "DicomAEServers.xml";

        public DicomServerTree DicomServerTree
        {
            get { return _dicomServerTree; }
            set { _dicomServerTree = value; }
        }

        public AEServerGroup SelectedServers
        {
            get { return _selectedServers; }
            set { _selectedServers = value; }
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

        // to delete
        private AEServerPool _serverPool;
        private ServerViewRootNode _serverTreeView;

        private static String _newServerName = "NewServer";
        private static String _emptyNodeName = "emptynode";
        private static String _nodeDeleted = "nodeDeleted";

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        public ServerViewRootNode ServerTreeView
        {
            get { return _serverTreeView; }
            set { _serverTreeView = value; }
        }

        public static String NewServerName
        {
            get { return AENavigatorComponent._newServerName; }
        }

        public static String EmptyNodeName
        {
            get { return AENavigatorComponent._emptyNodeName; }
        }

        public static String NodeDeleted
        {
            get { return AENavigatorComponent._nodeDeleted; }
        }

        #endregion

        public AENavigatorComponent()
        {
            _selectedServers = new AEServerGroup();
            _dicomServerTree = new DicomServerTree();
            if (_dicomServerTree.CurrentServer != null && _dicomServerTree.CurrentServer.IsServer)
            {
                _selectedServers.Servers.Add((DicomServer)_dicomServerTree.CurrentServer);
                _selectedServers.GroupID = _dicomServerTree.CurrentServer.GroupID;
                _selectedServers.Name = _dicomServerTree.CurrentServer.ServerName;
            }

        }

        public IDicomServer AddEditServer(IDicomServer dataNode)
        {
            if (dataNode == null)
                return null;
            _dicomServerTree.CurrentServer = dataNode;
            DicomServerEditComponent editor = new DicomServerEditComponent(_dicomServerTree);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Add New Server");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                SetSelectedServer((DicomServer)_dicomServerTree.CurrentServer);
                return _dicomServerTree.CurrentServer;
            }
            return null;
        }

        public IDicomServer AddEditServerGroup(IDicomServer dataNode, bool newGroup)
        {
            if (dataNode.IsServer)
                return null;
            _dicomServerTree.CurrentServer = (DicomServerGroup)dataNode;
            _dicomServerTree.IsMarked = newGroup;
            string title = _dicomServerTree.IsMarked ? "Add New Server Group" : "Edit Server Group";
            DicomServerGroupEditComponent editor = new DicomServerGroupEditComponent(_dicomServerTree);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, title);

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                SelectChanged(_dicomServerTree.CurrentServer);
                return _dicomServerTree.CurrentServer;
            }
            return null;
        }

        public void CEchoServer()
        {
            if (_selectedServers.Servers.Count == 0)
            {
                Platform.ShowMessageBox("There are no servers selected. Please select servers and try again.", MessageBoxActions.Ok);
                return;
            }
            LocalAESettings myAESettings = new LocalAESettings();
            ApplicationEntity myAE = new ApplicationEntity(new HostName("localhost"), new AETitle(myAESettings.AETitle), new ListeningPort(myAESettings.Port));
            StringBuilder msgText = new StringBuilder();
            msgText.AppendFormat("C-ECHO Verification:\r\n\r\n");
            using (DicomClient client = new DicomClient(myAE))
            {
                foreach (DicomServer ae in _selectedServers.Servers)
                {
                    if (client.Verify(ae.DicomAE))
                        msgText.AppendFormat("    {0}: successful    \r\n", ae.GroupID);
                    else
                        msgText.AppendFormat("    {0}: fail    \r\n", ae.GroupID);
                }
            }
            msgText.AppendFormat("\r\n");
            Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
            return;
        }

        public bool DeleteServer(IDicomServer dataNode)
        {
            if (!ServerDeleteConfirm(dataNode))
                return false;

            _dicomServerTree.CurrentServer = _dicomServerTree.RemoveDicomServer(dataNode);
            if (_dicomServerTree.CurrentServer == null)
                return false;
            _selectedServers.Servers = _dicomServerTree.FindChildServers(_dicomServerTree.CurrentServer, true);
            _selectedServers.GroupID = _dicomServerTree.CurrentServer.GroupID;
            _selectedServers.Name = _dicomServerTree.CurrentServer.ServerName;
            _dicomServerTree.SaveDicomServers();
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            return true;
        }

        private void SetSelectedServer(DicomServer server)
        {
            _selectedServers = new AEServerGroup();
            _selectedServers.Servers.Add(server);
            _selectedServers.GroupID = server.GroupID;
            _selectedServers.Name = server.ServerName;
            _dicomServerTree.CurrentServer = server;
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
        }

        private bool ServerDeleteConfirm(IDicomServer dataNode)
        {
            string msg = "";
            if (dataNode.IsServer)
                msg = "Are you sure you want to delete this server?";
            else
                msg = "Are you sure you want to delete this server group?";
            if (this.Host.ShowMessageBox(msg, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                return true;
            return false;
        }

        public void SelectChanged(IDicomServer dataNode)
        {
            if (dataNode.IsServer)
            {
                SetSelectedServer((DicomServer)dataNode);
            }
            else
            {
                _selectedServers = new AEServerGroup();
                _selectedServers.Servers = _dicomServerTree.FindChildServers((DicomServerGroup)dataNode, true);
                _selectedServers.GroupID = dataNode.GroupID;
                _selectedServers.Name = dataNode.ServerName;
                _dicomServerTree.CurrentServer = dataNode;
                EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            }

        }

        public event EventHandler SelectedServerChanged
        {
            add { _selectedServerChanged += value; }
            remove { _selectedServerChanged -= value; }
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

    }

}
