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

        public void Add()
        {
            // to do
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

        public void Delete()
        {
            // to do 
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

            int iport = 0;
            try
            {
                iport = int.Parse(this.ServerPort);
            }
            catch
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Port value ({0}) should be a integer. \r\n\r\nPlease input an integer data.", this.ServerPort);
                MessageBox.Show(msgText.ToString(), "Port Value Error");
                return false;
            }

            _serverPool.Currentserver = new AEServer(this.ServerName, _serverPool.Currentserver.Serverpath, this.ServerDesc, this.ServerHost, this.ServerAE, iport);
            _serverPool.Serverlist[_serverPool.Currentserverid] = _serverPool.Currentserver;
            _serverPool.SaveServerSettings();
            return true;
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
            /*_studyList = new TableData<StudyItem>();

            AddColumns();

            _toolSet = new ToolSet(new StudyBrowserToolExtensionPoint(), new StudyBrowserToolContext(this));
            _toolbarModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-toolbar", _toolSet.Actions);
            _contextMenuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "dicomstudybrowser-contextmenu", _toolSet.Actions);
            */
        }

        public override void Stop()
        {
            base.Stop();
        }

        #endregion

    }
}
