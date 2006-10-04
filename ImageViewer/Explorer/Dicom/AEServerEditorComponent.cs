using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    /// <summary>
    /// Extension point for views onto <see cref="AEServerEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class AEServerEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// AEServerEditorComponent class
    /// </summary>
    [AssociateView(typeof(AEServerEditorComponentViewExtensionPoint))]
    public class AEServerEditorComponent : ApplicationComponent
    {
        /// <summary>
        /// Constructor
        /// </summary>
        #region Fields

        private AEServerPool _serverPool;
        private string _serverName = "";
        private string _serverPath = "";
        private string _serverLocation = "";
        private string _serverAE = "";
        private string _serverHost = "";
        private string _serverPort = "";


        public AEServerPool ServerPool
        {
            get { return _serverPool; }
            set { _serverPool = value; }
        }

        public string ServerPath
        {
            get { return _serverPath; }
            set { 
                _serverPath = value;
                this.Modified = true;
            }
        }

        public string ServerName
        {
            get { return _serverName; }
            set 
            { 
                _serverName = value;
                if(!IsServerSettingValid())
                    this.Modified = false;
                else
                    this.Modified = true;
            }
        }

        public string ServerLocation
        {
            get { return _serverLocation; }
            set { 
                _serverLocation = value;
                this.Modified = true;
            }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { 
                _serverAE = value;
                if(!IsServerSettingValid())
                    this.Modified = false;
                else
                    this.Modified = true;
            }
        }

        public string ServerHost
        {
            get { return _serverHost; }
            set { 
                _serverHost = value;
                if(!IsServerSettingValid())
                    this.Modified = false;
                else
                    this.Modified = true;
            }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { 
                _serverPort = value;
                if(!IsServerSettingValid())
                    this.Modified = false;
                else
                    this.Modified = true;
            }
        }

        #endregion

        public AEServerEditorComponent(AEServerPool svrPool)
        {
            _serverPool = svrPool;
            if (_serverPool == null)
                return;
            if (_serverPool.Currentserver != null)
            {
                _serverName = _serverPool.Currentserver.Servername;
                _serverPath = _serverPool.Currentserver.Serverpath;
                _serverLocation = _serverPool.Currentserver.Serverlocation;
                _serverAE = _serverPool.Currentserver.AE;
                _serverHost = _serverPool.Currentserver.Host;
                _serverPort = _serverPool.Currentserver.Port.ToString();
            }
            else
            {
                _serverName = "";
                _serverPath = "";
                _serverLocation = "";
                _serverAE = "";
                _serverHost = "";
                _serverPort = "";
            }
        }

        public void Accept()
        {
            // edit current server
            if (_serverPool.Currentserver != null && _serverPool.Currentserverid >= 0 && _serverPool.Currentserverid < _serverPool.Serverlist.Count)
            {
                AEServer ae = new AEServer(_serverName, _serverPath, _serverLocation, _serverHost, _serverAE, int.Parse(_serverPort));
                ae.ServerID = _serverPool.Currentserverid;
                ae.ServerParentID = _serverPool.Currentserver.ServerParentID;
                ae.ServerPathGroups = _serverPool.Currentserver.ServerPathGroups;
                _serverPool.Currentserver = ae;
                _serverPool.Serverlist[_serverPool.Currentserverid] = _serverPool.Currentserver;
                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
                return;
            }
            // add new server
            if (_serverPool.Currentserver == null && _serverPool.Currentserverid >= 0 && _serverPool.Currentserverid < _serverPool.Serverlist.Count)
            {
                _serverPool.Currentserver = _serverPool.Serverlist[_serverPool.Currentserverid];
                AEServer ae = new AEServer(_serverName, _serverPath, _serverLocation, _serverHost, _serverAE, int.Parse(_serverPort));
                ae.ServerParentID = _serverPool.Currentserverid;
                _serverPool.Currentserverid = _serverPool.Serverlist.Count;
                ae.ServerID = _serverPool.Currentserverid;
                ae.Serverpath = _serverPool.Currentserver.Serverpath;
                ae.ServerPathGroups = null;
                _serverPool.Currentserver = ae;
                _serverPool.Serverlist.Add(_serverPool.Currentserver);
                this.ExitCode = ApplicationComponentExitCode.Normal;
                Host.Exit();
                return;
            }
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private bool IsServerSettingValid()
        {
            if (_serverName == null || _serverName.Equals("") || _serverAE == null || _serverAE.Equals("")
                || _serverHost == null || _serverHost.Equals("") || _serverPort == null || _serverPort.Equals(""))
            {
                return false;
            }

            int svrid = _serverPool.Currentserverid;
            if (_serverPool.Currentserver == null)
                svrid = _serverPool.Serverlist.Count;

            for (int i = 0; i < _serverPool.Serverlist.Count; i++)
            {
                if (i == svrid || _serverPool.Serverlist[i].ServerParentID < 0 
                    || _serverPool.Serverlist[i].Servername.Equals(AENavigatorComponent.EmptyNodeName))
                    continue;
                if (_serverName.Equals(_serverPool.Serverlist[i].Servername))
                {
                    ServerSettingError(ServerSettingItem.ServerName, -1000000 - i);
                    return false;
                }
            }

            for (int i = 0; i < _serverPool.Serverlist.Count; i++)
            {
                if (i == svrid || _serverPool.Serverlist[i].ServerParentID < 0
                    || _serverPool.Serverlist[i].Servername.Equals(AENavigatorComponent.EmptyNodeName))
                    continue;
                if (_serverAE.Equals(_serverPool.Serverlist[i].AE))
                {
                    ServerSettingError(ServerSettingItem.AE, -1000000 - i);
                    return false;
                }
            }

            try
            {
                int.Parse(_serverPort);
            }
            catch
            {
                ServerSettingError(ServerSettingItem.Port, -2);
                return false;
            }

            return true;
        }

        private void ServerSettingError(ServerSettingItem sitem, int errorvalue)
        {
            if (errorvalue == -1)
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The value of {0} should not be blank. \r\n\r\nPlease try again.", sitem.ToString());
                Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
            }
            else if (errorvalue == -2)
            {
                StringBuilder msgText = new StringBuilder();
                msgText.AppendFormat("The Port value should be a integer. \r\n\r\nPlease input an integer data.");
                Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
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
                        Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
                        break;
                    case ServerSettingItem.AE:
                        msgText.AppendFormat("The AE Title ({0}) exists under {1}\r\n\r\nServer Name: {2}\r\nAE Title: {3}\r\nHost: {4}\r\n\r\nPlease choose another AE Title.",
                            _serverPool.Serverlist[i].AE, _serverPool.Serverlist[i].Serverpath, _serverPool.Serverlist[i].Servername,
                            _serverPool.Serverlist[i].AE, _serverPool.Serverlist[i].Host);
                        Platform.ShowMessageBox(msgText.ToString(), MessageBoxActions.Ok);
                        break;
                    default:
                        break;
                }
            }
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public bool FieldReadonly
        {
            get 
            {
                if (_serverName.Equals(AENavigatorComponent.MyDatastoreTitle))
                    return true;
                return false; 
            }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }
    }

    public enum ServerSettingItem
    {
        ServerName,
        AE,
        Host,
        Port
    }

}
