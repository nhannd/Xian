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
        public void Update()
        {
            UpdateServerSetting();
        }

        public void ClearServerSetting()
        {
            this.ServerName = "";
            this.ServerPath = "";
            this.ServerDesc = "";
            this.ServerAE = "";
            this.ServerHost = "";
            this.ServerPort = "";
        }

        public static bool IsInteger(string theValue)
        {
            try
            {
                Convert.ToInt32(theValue);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void LoadServerSetting()
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

        public bool UpdateServerSetting()
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

            if (!IsInteger(this.ServerPort))
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Port value ({0}) should be a integer. \r\n\r\nPlease input an integer data.", this.ServerPort);
                MessageBox.Show(msgText.ToString(), "Port Value Error");
                return false;
            }


            // to do: check valid value of _textPort.Text first
            _serverPool.Currentserver = new AEServer(this.ServerName, _serverPool.Currentserver.Serverpath, this.ServerDesc, this.ServerHost, this.ServerAE, int.Parse(this.ServerPort));
            _serverPool.Serverlist[_serverPool.Currentserverid] = _serverPool.Currentserver;
            _serverPool.SaveServerSettings();
            //ResetTreeNode(_lastClickedNode, _serverPool.Currentserver);
            return true;
        }


        #region IApplicationComponent overrides

        public override void Start()
        {
            base.Start();

            _serverPool = new AEServerPool();
            _serverPool.Currentserver = _serverPool.Serverlist[0];
            _serverPool.Currentserverid = 0;
            LoadServerSetting();
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

        #region Fields

        private AEServerPool _serverPool;
        //private ContextMenuStrip _treeContextMenu;
        //private ContextMenuStrip _nodeContextMenu;
        //private ContextMenuStrip _groupContextMenu;
        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My DataStore";
        private static String _emptyNodeName = "emptynode";

        private string _serverName = "";
        private string _serverPath = "";
        private string _serverDesc = "";
        private string _serverAE = "";
        private string _serverHost = "";
        private string _serverPort = "";

        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
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
    }
}
