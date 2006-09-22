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

        private AEServerPool _serverPool;
        private AEServer _serverSelected;
        private List<AEServer> _selectedServers;
        private ServerViewRootNode _serverTreeView;
        private event EventHandler _selectedServerChanged;

        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My DataStore";
        private static String _newServerName = "NewServer";
        private static String _emptyNodeName = "emptynode";

        private string _activeNode;
        private string _activePath;

        private string _serverName = "";
        private string _serverPath = "";
        private string _serverDesc = "";
        private string _serverAE = "";
        private string _serverHost = "";
        private string _serverPort = "";

        public string ActiveNode
        {
            get { return _activeNode; }
            set { _activeNode = value; }
        }

        public string ActivePath
        {
            get { return _activePath; }
            set { _activePath = value; }
        }

        public event EventHandler SelectedServerChanged
        {
            add { _selectedServerChanged += value; }
            remove { _selectedServerChanged -= value; }
        }

        public AEServer ServerSelected
        {
            get { return _serverSelected; }
            set { _serverSelected = value; }
        }

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        public List<AEServer> SelectedServers
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

        public static String NewServerName
        {
            get { return AENavigatorComponent._newServerName; }
            set { AENavigatorComponent._newServerName = value; }
        }

        public static String EmptyNodeName
        {
            get { return AENavigatorComponent._emptyNodeName; }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { _serverPath = value; }
        }

        public string ServerName
        {
            get { return _serverName; }
            set { _serverName = value; }
        }

        public string ServerDesc
        {
            get { return _serverDesc; }
            set { _serverDesc = value; }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { _serverAE = value; }
        }

        public string ServerHost
        {
            get { return _serverHost; }
            set { _serverHost = value; }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { _serverPort = value; }
        }

        #endregion

        public void AddServer()
        {
            //UpdateServerSetting();
            //_serverPool.SetNewServer(NewServerName, tstamp);
            //LoadServerSetting();
            //_serverSelected = _serverPool.Currentserver;
            //EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            //return NewServerName+tstamp;
        }

        public bool DeleteServer()
        {
            if (_serverPool.Currentserver == null || _serverPool.Currentserverid < 0 || _serverPool.Currentserverid >= _serverPool.Serverlist.Count)
                return false;
            _serverPool.Serverlist.RemoveAt(_serverPool.Currentserverid);
            _serverPool.Currentserver = null;
            _serverPool.Currentserverid = -1;
            _serverPool.SaveServerSettings();
            LoadServerSetting();
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            return true;
        }

        public void UpdateServer()
        {
            AEServerEditorComponent editor = new AEServerEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Update AE Server...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                return;
            }
        }

        public void SelectChanged(IBrowserNode dataNode)
        {
            //UpdateServerSetting();
            if (dataNode.IsServerNode)
            {
                _selectedServers = new List<AEServer>();
                _selectedServers.Add(_serverPool.Serverlist[dataNode.ServerID]);
            }
            else
            {
                _selectedServers = _serverPool.GetChildServers(dataNode.ServerID, true, true);
            }
            //LoadServerSetting();
            //_serverSelected = _serverPool.Currentserver;
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
        }

        public void Update()
        {
            UpdateServerSetting();
        }

        private void ClearServerSetting()
        {
            this.ServerName = "";
            this.ServerPath = "";
            this.ServerDesc = "";
            this.ServerAE = "";
            this.ServerHost = "";
            this.ServerPort = "";
        }

        private void LoadServerSetting()
        {
            if (_serverPool == null)
                _serverPool = new AEServerPool();
            if (_serverPool.Currentserver == null)
            {
                ClearServerSetting();
            }
            else if (_serverPool.Currentserver.Servername.Equals(EmptyNodeName))
            {
                _serverPool.Currentserver = null;
                _serverPool.Currentserverid = -1;
                ClearServerSetting();
            }
            else
            {
                this.ServerName = _serverPool.Currentserver.Servername;
                this.ServerDesc = _serverPool.Currentserver.Serverlocation;
                this.ServerAE = _serverPool.Currentserver.AE;
                this.ServerHost = _serverPool.Currentserver.Host;
                this.ServerPort = _serverPool.Currentserver.Port.ToString();
            }
        }

        private bool UpdateServerSetting()
        {
            //update server setting
            if (_serverPool.Currentserver == null || _serverPool.Serverlist == null
               || _serverPool.Currentserverid < 0
               || _serverPool.Currentserverid >= _serverPool.Serverlist.Count)
                return true;
            if (_serverPool.Currentserver.Servername.Equals(this.ServerName)
                    && _serverPool.Currentserver.Serverlocation.Equals(this.ServerDesc)
                    && _serverPool.Currentserver.AE.Equals(this.ServerAE)
                    && _serverPool.Currentserver.Host.Equals(this.ServerHost)
                    && _serverPool.Currentserver.Port.ToString().Equals(this.ServerPort))
                return true;

            _serverPool.Currentserver = new AEServer(this.ServerName, _serverPool.Currentserver.Serverpath, this.ServerDesc, this.ServerHost, this.ServerAE, int.Parse(this.ServerPort));
            _serverPool.Serverlist[_serverPool.Currentserverid] = _serverPool.Currentserver;
            _serverPool.SaveServerSettings();
            return true;
        }

        public void ServerSettingError(ServerSettingItem sitem, int errorvalue)
        {
            // to do
        }

        public bool ServerDeleteConfirm()
        {
            DialogResult dr = MessageBox.Show("Do you really want to remove this server?", "Remove Server", MessageBoxButtons.YesNo);
            if (dr == DialogResult.Yes)
                return true;
            return false;
        }

        public int IsServerSettingValid(ServerSettingItem sitem, string svalue, bool chkchangeonly)
        {
            // to do
            return 0;
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            _serverPool = new AEServerPool();
            _serverPool.LoadServerSettings();
            _serverPool.Currentserver = null;
            _serverPool.Currentserverid = -1;
            ClearServerSetting();
            _serverTreeView = new ServerViewRootNode(_serverPool);
        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

    }

    public enum ServerSettingItem
    {
        ServerName,
        ServerPath,
        Descrition,
        AE,
        Host,
        Port
    }

}
