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
        private ServerViewRootNode _serverTreeView;
        private event EventHandler _selectedServerChanged;
        private event EventHandler _dataStoreSelected;

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

        public event EventHandler DataStoreSelected
        {
            add { _dataStoreSelected += value; }
            remove { _dataStoreSelected -= value; }
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

        public string AddServer()
        {
            Random r = new Random();
            string tstamp = r.Next(10000).ToString();
            UpdateServerSetting();
            _serverPool.SetNewServer(NewServerName, tstamp);
            LoadServerSetting();
            _serverSelected = _serverPool.Currentserver;
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
            return NewServerName+tstamp;
        }

        public void DataStoreEvent()
        {
            UpdateServerSetting();
            _serverPool.Currentserver = null;
            _serverPool.Currentserverid = -1;
            _serverSelected = null;
            LoadServerSetting();
            EventsHelper.Fire(_dataStoreSelected, this, EventArgs.Empty);
            EventsHelper.Fire(_selectedServerChanged, this, EventArgs.Empty);
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

        public void SelectChanged(String nodeName)
        {
            UpdateServerSetting();
            _serverPool.SetCurrentServerByName(nodeName);
            LoadServerSetting();
            _serverSelected = _serverPool.Currentserver;
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
                this.ServerDesc = _serverPool.Currentserver.Description;
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
                    && _serverPool.Currentserver.Description.Equals(this.ServerDesc)
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
            if(errorvalue == -1)
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The value of {0} should not be blank. \r\n\r\nPlease try again.", sitem.ToString());
                MessageBox.Show(msgText.ToString(), "Blank Value Error");
            }
            else if (errorvalue == -2)
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Port value should be a integer. \r\n\r\nPlease input an integer data.");
                MessageBox.Show(msgText.ToString(), "Port Value Error");
            }
            else if (errorvalue <= -1000000)
            {
                int i = (-1 * errorvalue) - 1000000;
                if (i < 0 || i >= _serverPool.Serverlist.Count || i == _serverPool.Currentserverid)
                    return;
                StringBuilder msgText = new StringBuilder();
                switch (sitem)
                {
                    case ServerSettingItem.ServerName:
                        msgText.AppendFormat("The Saver Name ({0}) exists under {1}\r\n\r\nServer Name: {2}\r\nAE Title: {3}\r\nHost: {4}\r\n\r\nPlease choose another server name.",
                            _serverPool.Serverlist[i].Servername, _serverPool.Serverlist[i].Serverpath, _serverPool.Serverlist[i].Servername,
                            _serverPool.Serverlist[i].AE, _serverPool.Serverlist[i].Host);
                        MessageBox.Show(msgText.ToString(), "Server Name Error");
                        break;
                    case ServerSettingItem.AE:
                        msgText.AppendFormat("The AE Title ({0}) exists under {1}\r\n\r\nServer Name: {2}\r\nAE Title: {3}\r\nHost: {4}\r\n\r\nPlease choose another server name.",
                            _serverPool.Serverlist[i].AE, _serverPool.Serverlist[i].Serverpath, _serverPool.Serverlist[i].Servername,
                            _serverPool.Serverlist[i].AE, _serverPool.Serverlist[i].Host);
                        MessageBox.Show(msgText.ToString(), "AE Title Error");
                        break;
                    case ServerSettingItem.Host:
                        msgText.AppendFormat("The Saver Host ({0}) exists under {1}\r\n\r\nServer Name: {2}\r\nAE Title: {3}\r\nHost: {4}\r\n\r\nPlease choose another server name.",
                            _serverPool.Serverlist[i].Host, _serverPool.Serverlist[i].Serverpath, _serverPool.Serverlist[i].Servername,
                            _serverPool.Serverlist[i].AE, _serverPool.Serverlist[i].Host);
                        MessageBox.Show(msgText.ToString(), "Server Host Error");
                        break;
                    default:
                        break;
                }
            }
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
            if (_serverPool.Currentserver == null)
                return 1;

            if (svalue == null || svalue.Equals(""))
            {
                switch (sitem)
                {
                    case ServerSettingItem.ServerName:
                        this.ServerName = _serverPool.Currentserver.Servername;
                        break;
                    case ServerSettingItem.AE:
                        this.ServerAE = _serverPool.Currentserver.AE;
                        break;
                    case ServerSettingItem.Host:
                        this.ServerHost = _serverPool.Currentserver.Host;
                        break;
                    case ServerSettingItem.Port:
                        this.ServerPort = _serverPool.Currentserver.Port.ToString();
                        break;
                    default:
                        throw new System.Exception("Unexpected condition in figuring out which server setting item was updated");
                        //return false;
                }
                return -1;
            }

            switch (sitem)
            {
                case ServerSettingItem.ServerName:
                    if (chkchangeonly && this.ServerName.Equals(_serverPool.Currentserver.Servername))
                        return 2;
                    for (int i = 0; i < _serverPool.Serverlist.Count; i++)
                    {
                        if (i == _serverPool.Currentserverid || _serverPool.Serverlist[i].Servername.Equals(EmptyNodeName))
                            continue;
                        if (this.ServerName.Equals(_serverPool.Serverlist[i].Servername))
                        {
                            this.ServerName = _serverPool.Currentserver.Servername;
                            return -1000000 - i;
                        }
                    }
                    return 3;
                case ServerSettingItem.AE:
                    if (chkchangeonly && this.ServerAE.Equals(_serverPool.Currentserver.AE))
                        return 4;
                    for (int i = 0; i < _serverPool.Serverlist.Count; i++)
                    {
                        if (i == _serverPool.Currentserverid || _serverPool.Serverlist[i].Servername.Equals(EmptyNodeName))
                            continue;
                        if (this.ServerAE.Equals(_serverPool.Serverlist[i].AE))
                        {
                            this.ServerAE = _serverPool.Currentserver.AE;
                            return -1000000 - i;
                        }
                    }
                    return 5;
                case ServerSettingItem.Host:
                    if (chkchangeonly && this.ServerHost.Equals(_serverPool.Currentserver.Host))
                        return 6;
                    for (int i = 0; i < _serverPool.Serverlist.Count; i++)
                    {
                        if (i == _serverPool.Currentserverid || _serverPool.Serverlist[i].Servername.Equals(EmptyNodeName))
                            continue;
                        if (this.ServerHost.Equals(_serverPool.Serverlist[i].Host))
                        {
                            this.ServerHost = _serverPool.Currentserver.Host;
                            return -1000000 - i;
                        }
                    }
                    return 7;
                case ServerSettingItem.Port:
                    try
                    {
                        int.Parse(this.ServerPort);
                    }
                    catch
                    {
                        this.ServerPort = _serverPool.Currentserver.Port.ToString();
                        return -2;
                    }
                    return 8;
                default: 
                    //throw new System.Exception("Unexpected condition in figuring out which server setting item was updated");
                    return -3;
            }
        }

        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            _serverPool = new AEServerPool();
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
