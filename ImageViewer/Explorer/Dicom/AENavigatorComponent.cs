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

        private DicomServerGroup _dicomServerGroup;
        private event EventHandler _selectedServerChanged;

        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My Studies";
        private static String _myServersRoot = "MyServersRoot";

        private AEServerPool _serverPool;
        private AEServerGroup _selectedServers;
        private ServerViewRootNode _serverTreeView;

        // to delete
        private static String _newServerName = "NewServer";
        private static String _emptyNodeName = "emptynode";
        private static String _nodeDeleted = "nodeDeleted";

        public DicomServerGroup DicomServerGroup
        {
            get { return _dicomServerGroup; }
            set { _dicomServerGroup = value; }
        }

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        public AEServerGroup SelectedServers
        {
            get { return _selectedServers; }
            set { _selectedServers = value; }
        }

        public ServerViewRootNode ServerTreeView
        {
            get { return _serverTreeView; }
            set { _serverTreeView = value; }
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
			_serverPool = new AEServerPool();
			_serverPool.LoadServerSettings();
			_serverPool.Currentserverid = -1;
			_serverPool.Currentserver = _serverPool.GetMyDatastoreServer();
			if (_serverPool.Currentserver != null)
			{
				_selectedServers.Servers.Add(_serverPool.Currentserver);
				_selectedServers.GroupID = _serverPool.Currentserver.getServerGroupID();
				_selectedServers.Name = _serverPool.Currentserver.Servername;
				_serverPool.Currentserverid = _serverPool.Currentserver.ServerID;
			}
			_serverTreeView = new ServerViewRootNode(_serverPool);

            _dicomServerGroup = DicomServerService.LoadDicomServers();

		}

        public ServerViewServerNode AddServer(IBrowserNode dataNode)
        {
            if (dataNode.IsServerNode || dataNode.ServerID < 0 || dataNode.ServerID >= _serverPool.Serverlist.Count)
                return null;
            _serverPool.Currentserver = null;
            _serverPool.Currentserverid = dataNode.ServerID;
            AEServerEditorComponent editor = new AEServerEditorComponent(_serverPool);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Add New Server");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _serverPool.SaveServerSettings();
				AEServer server = _serverPool.Serverlist[_serverPool.Currentserverid];
				SetSelectedServer(server);
                return (new ServerViewServerNode(_serverPool.Currentserver, _serverPool));
            }
            return null;
        }

        public ServerViewServerNode AddServerGroup(IBrowserNode dataNode)
        {
            if (dataNode.IsServerNode || dataNode.ServerID < 0 || dataNode.ServerID >= _serverPool.Serverlist.Count)
                return null;
            _serverPool.Currentserver = null;
            _serverPool.Currentserverid = dataNode.ServerID;
            AEServerGroupEditorComponent editor = new AEServerGroupEditorComponent(_serverPool);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Add New Server Group");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _serverPool.SaveServerSettings();
                ServerViewServerNode newDataNode = new ServerViewServerNode(_serverPool.Currentserver, _serverPool);
                SelectChanged(newDataNode);
                return newDataNode;
            }
            return null;
        }

        public ServerViewServerNode EditServer(IBrowserNode dataNode)
        {
            if (dataNode.ServerID < 0 || dataNode.ServerID >= _serverPool.Serverlist.Count)
                return null;
            if (dataNode.IsServerNode)
            {
                _serverPool.SetCurrentServerByID(dataNode.ServerID);
                AEServerEditorComponent editor = new AEServerEditorComponent(_serverPool);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Edit Server");
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _serverPool.SaveServerSettings();

				    AEServer server = _serverPool.Serverlist[_serverPool.Currentserverid];
				    SetSelectedServer(server);
    				
				    return (new ServerViewServerNode(_serverPool.Currentserver, _serverPool));
                }
            }
            else
            {
                _serverPool.SetCurrentServerByID(dataNode.ServerID);
                AEServerGroupEditorComponent editor = new AEServerGroupEditorComponent(_serverPool);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Edit Server Group");
                if (exitCode == ApplicationComponentExitCode.Normal)
                {
                    _serverPool.SaveServerSettings();

                    ServerViewServerNode newDataNode = new ServerViewServerNode(_serverPool.Currentserver, _serverPool);
                    SelectChanged(newDataNode);
                    return newDataNode;
                }
            }
            return null;
        }

        public bool DeleteServer(IBrowserNode dataNode)
        {
            if (!ServerDeleteConfirm(dataNode))
                return false;

            _serverPool.Currentserverid = _serverPool.Serverlist[dataNode.ServerID].ServerParentID;
            _serverPool.Currentserver = null;
            if (dataNode.IsServerNode)
            {
                _serverPool.Serverlist[dataNode.ServerID].Servername = NodeDeleted;
            }
            else
            {
                _selectedServers.Servers = _serverPool.GetChildServers(dataNode.ServerID, false, true);
                foreach (AEServer ae in _selectedServers.Servers)
                {
                    _serverPool.Serverlist[ae.ServerID].Servername = NodeDeleted;
                    _serverPool.Serverlist[ae.ServerID].ServerParentID = -1;
                }
                _serverPool.Serverlist[dataNode.ServerID].Servername = NodeDeleted;
                _serverPool.Serverlist[dataNode.ServerID].ServerParentID = -1;
            }
            _selectedServers.Servers = _serverPool.GetChildServers(_serverPool.Currentserverid, true, true);
            _selectedServers.GroupID = _serverPool.Serverlist[_serverPool.Currentserverid].getServerGroupID();
            _selectedServers.Name = _serverPool.Serverlist[_serverPool.Currentserverid].getServerGroupName();
            _serverPool.Currentserverid = -1;
            _serverPool.SaveServerSettings();
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            return true;
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
                _selectedServers.Servers.Sort(delegate(AEServer s1, AEServer s2) { return s1.Serverpath.CompareTo(s2.Serverpath); });
                foreach (AEServer ae in _selectedServers.Servers)
                {
                    ApplicationEntity me = ae.getApplicationEntity();
                    if (client.Verify(me))
                        msgText.AppendFormat("    {0}: successful    \r\n", ae.Serverpath + "/" + ae.Servername);
                    else
                        msgText.AppendFormat("    {0}: fail    \r\n", ae.Serverpath + "/" + ae.Servername);
                }
            }
            msgText.AppendFormat("\r\n");
            Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
            return;
        }

        public void SelectChanged(IBrowserNode dataNode)
        {
            AEServer server = _serverPool.Serverlist[dataNode.ServerID];
			
			if (dataNode.IsServerNode)
            {
				SetSelectedServer(server);
			}
            else
            {
				_selectedServers = new AEServerGroup();
				_selectedServers.Servers = _serverPool.GetChildServers(dataNode.ServerID, true, true);
				_selectedServers.GroupID = server.getServerGroupID() + "/" + server.getServerGroupName();
                _selectedServers.Name = server.getServerGroupName();
				EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
			}

        }

        private void SetSelectedServer(AEServer server)
        {
            _selectedServers = new AEServerGroup();
            _selectedServers.Servers.Add(server);
            _selectedServers.GroupID = server.getServerGroupID() + "/" + server.Servername;
            _selectedServers.Name = server.Servername;
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
        }

        private bool ServerDeleteConfirm(IBrowserNode dataNode)
        {
            string msg = "";
            if (dataNode.IsServerNode)
                msg = "Are you sure you want to delete this server?";
            else
                msg = "Are you sure you want to delete this server group?";
            if (this.Host.ShowMessageBox(msg, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
                return true;
            return false;
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
